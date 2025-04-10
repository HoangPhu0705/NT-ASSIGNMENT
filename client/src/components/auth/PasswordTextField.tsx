import { useState } from "react";
import { ViewIcon, ViewOffIcon } from "hugeicons-react";

interface PasswordTextFieldProps {
  label: string;
  id: string;
  placeholder?: string;
  error?: string;
  value?: string;
  onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

export default function PasswordTextField({
  label,
  id,
  placeholder,
  error,
  value,
  onChange,
}: PasswordTextFieldProps) {
  const [showPassword, setShowPassword] = useState(false);

  const togglePasswordVisibility = () => {
    setShowPassword((prev) => !prev);
  };

  const inputType = showPassword ? "text" : "password";

  return (
    <div className="mb-4 mt-7">
      <div className="relative z-10">
        <input
          id={id}
          type={inputType}
          value={value}
          onChange={onChange}
          className="peer block w-full appearance-none border-0 border-b-2 border-gray-700 bg-transparent px-0 py-2.5 text-md text-gray-700 focus:border-black focus:border-b-3 focus:outline-none focus:ring-0"
          placeholder={placeholder || " "}
        />
        <label
          htmlFor={id}
          className="absolute top-3 -z-10 origin-[0] -translate-y-6 scale-85 transform text-md text-gray-500 duration-300 peer-placeholder-shown:translate-y-0 
          peer-placeholder-shown:scale-100 peer-focus:start-0 peer-focus:-translate-y-6 peer-focus:scale-75 
          rtl:peer-focus:left-auto rtl:peer-focus:translate-x-1/4"
        >
          {label}
        </label>
        <button
          type="button"
          onClick={togglePasswordVisibility}
          onMouseDown={(e) => e.preventDefault()}
          className="absolute right-0 top-2 text-gray-500 focus:outline-none hover:cursor-pointer"
        >
          {showPassword ? <ViewIcon size={25} /> : <ViewOffIcon size={25} />}
        </button>
      </div>

      {error && <p className="text-red-500 text-sm mt-1">{error}</p>}
    </div>
  );
}
