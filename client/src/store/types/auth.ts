export interface User {
  userId: string;
  userName: string;
  email: string;
  role: string;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  loading: boolean;
  error: string | null;
}

export interface AuthResponse {
  token: string;
  userId: string;
  userName: string;
  email: string;
  role: string;
}

export interface LoginCredentials {
  email: string;
  password: string;
}
