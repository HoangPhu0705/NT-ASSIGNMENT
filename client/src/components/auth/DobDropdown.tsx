import React from "react";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "../ui/select";

interface DobDropdownProps {
  placeholder: string;
  items: string[];
  value: string;
  onValueChange: (value: string) => void;
}

function DobDropdown({
  placeholder,
  items,
  value,
  onValueChange,
}: DobDropdownProps) {
  return (
    <Select onValueChange={onValueChange} value={value}>
      <SelectTrigger className="w-[110px]">
        <SelectValue placeholder={placeholder} />
      </SelectTrigger>
      <SelectContent>
        {items.map((item) => (
          <SelectItem key={item} value={item}>
            {item}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}

export default DobDropdown;
