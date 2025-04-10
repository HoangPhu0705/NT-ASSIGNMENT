import { useState } from "react";
import { format, getMonth, getYear, setMonth, setYear } from "date-fns";
import { Calendar03Icon } from "hugeicons-react";
import { cn } from "@/lib/utils";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";

import DobDropdown from "./DobDropdown";

interface DobTextFieldProps {
  label: string;
  id: string;
  placeholder?: string;
  error?: string;
  value?: Date | undefined;
  onChange?: (date: Date | undefined) => void;
}

export default function DobTextField({
  label,
  id,
  placeholder,
  error,
  value,
  onChange,
}: DobTextFieldProps) {
  const [open, setOpen] = useState(false);
  const [date, setDate] = useState<Date | undefined>(value || undefined);

  const months = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December",
  ];
  const currentYear = getYear(new Date());
  const years = Array.from({ length: 50 }, (_, i) => currentYear - i);

  const handleDateSelect = (selectedDate: Date | undefined) => {
    if (selectedDate && selectedDate > new Date()) {
      return;
    }
    setDate(selectedDate);
    if (onChange) onChange(selectedDate);
    setOpen(false);
  };

  const handleMonthChange = (month: string) => {
    const newDate = date
      ? setMonth(date, months.indexOf(month))
      : setMonth(new Date(), months.indexOf(month));
    if (newDate > new Date()) {
      setDate(new Date());
    } else {
      setDate(newDate);
    }
  };

  const handleYearChange = (year: string) => {
    const newDate = date
      ? setYear(date, parseInt(year))
      : setYear(new Date(), parseInt(year));
    if (newDate > new Date()) {
      setDate(new Date());
    } else {
      setDate(newDate);
    }
  };

  return (
    <div className="mb-4 mt-7">
      <div className="relative z-11">
        <Popover open={open} onOpenChange={setOpen}>
          <PopoverTrigger asChild>
            <input
              id={id}
              type="text"
              value={date ? format(date, "PPP") : ""}
              readOnly
              onClick={() => setOpen(true)}
              className={cn(
                "peer block w-full appearance-none border-0 border-b-2 border-gray-700 bg-transparent px-0 py-2.5 text-md text-gray-700 focus:border-black focus:border-b-3 focus:outline-none focus:ring-0",
                !date && "text-transparent peer-placeholder-shown:text-gray-700"
              )}
              placeholder={placeholder || " "}
            />
          </PopoverTrigger>
          <PopoverContent className="w-auto p-0" align="start" side="bottom">
            <div className="bg-white rounded-lg shadow-lg">
              <div className="flex justify-between p-2">
                <DobDropdown
                  placeholder="Month"
                  items={months}
                  value={
                    date ? months[getMonth(date)] : months[getMonth(new Date())]
                  }
                  onValueChange={handleMonthChange}
                />
                <DobDropdown
                  placeholder="Year"
                  items={years.map((year) => year.toString())}
                  value={
                    date
                      ? getYear(date).toString()
                      : getYear(new Date()).toString()
                  }
                  onValueChange={handleYearChange}
                />
              </div>
              <Calendar
                mode="single"
                selected={date}
                onSelect={handleDateSelect}
                initialFocus
                month={date || new Date()}
                onMonthChange={setDate}
                toDate={new Date()}
                classNames={{
                  day_selected: `bg-[#2F3132] border-[#2F3132] text-white`,
                }}
              />
            </div>
          </PopoverContent>
        </Popover>
        <label
          htmlFor={id}
          className="absolute top-3 -z-10 origin-[0] -translate-y-6 scale-85 transform text-md text-gray-500 duration-300 peer-placeholder-shown:translate-y-0 peer-placeholder-shown:scale-100"
        >
          {label}
        </label>
        <Calendar03Icon className="absolute right-0 top-2.5 h-5 w-5 text-gray-500" />
      </div>

      {error && <p className="text-red-500 text-sm mt-1">{error}</p>}
    </div>
  );
}
