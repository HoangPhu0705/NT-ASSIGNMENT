/* eslint-disable @typescript-eslint/no-explicit-any */
import { useState, useCallback } from "react";
import api from "@/lib/api";
import { AxiosRequestConfig, AxiosResponse } from "axios";

interface UseAxiosResponse<T> {
  loading: boolean;
  error: string | null;
  data: T | null;
  request: (config: AxiosRequestConfig) => Promise<T>;
}

const useAxios = <T = unknown>(): UseAxiosResponse<T> => {
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [data, setData] = useState<T | null>(null);

  const request = useCallback(
    async (config: AxiosRequestConfig): Promise<T> => {
      setLoading(true);
      setError(null);
      try {
        const response: AxiosResponse<T> = await api(config);
        setData(response.data);
        return response.data;
      } catch (err: any) {
        const message = err.response?.data?.message || "Request failed";
        setError(message);
        throw err;
      } finally {
        setLoading(false);
      }
    },
    []
  );

  return { loading, error, data, request };
};

export default useAxios;
