import { useAuth } from "oidc-react";
import axios, {
  AxiosInstance,
  AxiosResponse,
  InternalAxiosRequestConfig,
} from "axios";
import { useCallback, useEffect, useMemo, useState } from "react";
import { User } from "oidc-client-ts";

interface UseAxiosReturn {
  axiosInstance: AxiosInstance;
  isLoading: boolean;
  error: string | null;
}

const useAxios = (): UseAxiosReturn => {
  const auth = useAuth();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const axiosInstance = useMemo(() => {
    return axios.create({
      baseURL: import.meta.env.VITE_API_BASE_URL + "/api/admin", // https://localhost:7130
      headers: {
        "Content-Type": "application/json",
      },
    });
  }, []);

  // Refresh token logic
  const refreshToken = useCallback(async () => {
    if (!auth.userData?.refresh_token) {
      throw new Error("No refresh token available");
    }

    try {
      setIsLoading(true);
      const response = await axios.post(
        `${import.meta.env.VITE_API_BASE_URL}/connect/token`,
        new URLSearchParams({
          client_id: "admin-web-client",
          grant_type: "refresh_token",
          refresh_token: auth.userData.refresh_token,
          scope: "openid profile email roles api offline_access",
        }),
        {
          headers: {
            "Content-Type": "application/x-www-form-urlencoded",
          },
        }
      );

      const { access_token, refresh_token, expires_at, scope } = response.data;

      const user = new User({
        access_token,
        refresh_token,
        expires_at,
        scope: scope || "openid profile email roles api offline_access",
        token_type: "Bearer",
        profile: auth.userData?.profile || {},
        id_token: auth.userData?.id_token,
        session_state: auth.userData?.session_state || null,
      });

      // Store the updated user
      await auth.userManager.storeUser(user);

      return access_token;
    } catch (err: unknown) {
      auth.signIn();
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, [auth]);

  useEffect(() => {
    const requestInterceptor = axiosInstance.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        setIsLoading(true); // Set loading to true for every request
        if (auth.userData?.access_token) {
          config.headers = config.headers || {};
          config.headers.Authorization = `Bearer ${auth.userData.access_token}`;
        }
        return config;
      },
      (err) => {
        setIsLoading(false); // Reset loading on request error
        return Promise.reject(err);
      }
    );

    const responseInterceptor = axiosInstance.interceptors.response.use(
      (response: AxiosResponse) => {
        setIsLoading(false); // Reset loading on successful response
        return response;
      },
      async (err) => {
        setIsLoading(false); // Reset loading on response error
        const originalRequest = err.config;
        if (err.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;
          try {
            const newAccessToken = await refreshToken();
            originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
            return axiosInstance(originalRequest);
          } catch (refreshErr) {
            setError(err.message);
            return Promise.reject(refreshErr);
          }
        }
        setError(err.message);
        return Promise.reject(err);
      }
    );

    return () => {
      axiosInstance.interceptors.request.eject(requestInterceptor);
      axiosInstance.interceptors.response.eject(responseInterceptor);
    };
  }, [auth.userData, axiosInstance, refreshToken]);

  return { axiosInstance, isLoading, error };
};

export default useAxios;
