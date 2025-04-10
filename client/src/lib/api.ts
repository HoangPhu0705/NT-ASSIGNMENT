import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_BACKEND_API,
  headers: { "Content-Type": "application/json" },
});

api.interceptors.request.use(
  (config) => {
    // If you keep token in Redux state and need it in headers:
    // const token = store.getState().auth.token;
    // if (token && config.headers) {
    //   config.headers.Authorization = `Bearer ${token}`;
    // }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      console.error("Unauthorized - redirecting to login");
    }
    return Promise.reject(error);
  }
);

export default api;
