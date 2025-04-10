import EmailTextField from "@/components/auth/EmailTextField";
import PasswordTextField from "@/components/auth/PasswordTextField";
import { Button } from "@/components/ui/button";
import { Cancel01Icon } from "hugeicons-react";
import React from "react";
import { useDispatch } from "react-redux";
import { AppDispatch } from "@/store";
import { login } from "@/store/slices/authSlice";

function Login() {
  const dispatch = useDispatch<AppDispatch>();

  const handleEmailLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget as HTMLFormElement);
    const credentials = {
      email: formData.get("email") as string,
      password: formData.get("password") as string,
    };
    try {
      await dispatch(login(credentials)).unwrap();
    } catch (err) {
      console.error("Login error:", err);
    }
  };

  return (
    <div className="bg-[#F4F4F4] flex flex-col justify-center items-center w-screen min-h-screen px-4 sm:px-6 md:px-8 lg:px-16 xl:px-32">
      <h1 className="text-3xl sm:text-4xl md:text-6xl font-bold mb-3 flex items-center gap-1">
        NE
        <span className="inline-flex items-center mt-1 sm:mt-2">
          <Cancel01Icon size={36} className="sm:w-12 sm:h-12 md:w-14 md:h-14" />
        </span>
        TECH
      </h1>
      <p className="text-lg sm:text-xl md:text-2xl font-light mb-3 text-center">
        Login with your NEX ID.
      </p>
      <form
        onSubmit={handleEmailLogin}
        className="space-y-4 w-full max-w-md sm:max-w-md md:max-w-2xl lg:max-w-3xl"
      >
        <EmailTextField label="Email Address" id="email_floating" />
        <PasswordTextField label="Password" id="password_floating" />
        <div className="text-right">
          <a
            href="#"
            className="text-black text-sm sm:text-md font-bold hover:text-gray-500 underline"
          >
            Forgot password?
          </a>
        </div>
        <Button
          type="submit"
          className="w-full bg-[#2F3132] rounded-none text-primary-foreground border-3 hover:bg-transparent hover:text-black hover:cursor-pointer py-6 sm:py-7"
        >
          <span className="font-bold text-lg sm:text-xl md:text-xl">LOGIN</span>
        </Button>
      </form>
      <div className="flex flex-col sm:flex-row items-center gap-2 mt-4 text-center">
        <p className="text-md sm:text-lg">Don't have an account?</p>
        <a
          href="/auth/create-account"
          className="text-[#2F3132] text-md sm:text-lg hover:text-gray-500 font-bold hover:underline mb-0.5 underline"
        >
          Create Account
        </a>
      </div>
    </div>
  );
}

export default Login;
