import React from "react";

interface RegularTextFieldProps {
  label: string;
  id: string;
  placeholder?: string;
  error?: string;
  value?: string;
  onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

export default function RegularTextField({
  label,
  id,
  placeholder,
  error,
  value,
  onChange,
}: RegularTextFieldProps) {
  return (
    <div className="mb-4 mt-7">
      <div className="relative z-10">
        <input
          id={id}
          type="text"
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
      </div>

      {error && <p className="text-red-500 text-sm mt-1">{error}</p>}
    </div>
  );
}
