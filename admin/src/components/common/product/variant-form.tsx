/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState, useEffect } from "react";
import {
  Card,
  CardContent,
  Button,
  Label,
  Input,
  AccordionItem,
  AccordionTrigger,
  AccordionContent,
} from "@/components/ui";
import AttributeForm from "./attribute-form";
import toast from "react-hot-toast";
import { AxiosInstance } from "axios";

interface VariantAttribute {
  name: string;
  value: string;
}

interface Variant {
  id?: string;
  name: string;
  sku: string;
  price: number;
  stock: number;
  attributes: VariantAttribute[];
}

interface VariantFormProps {
  variant: Variant;
  index: number;
  isSingleVariant: boolean;
  isLoading: boolean;
  productId?: string;
  axiosInstance: AxiosInstance;
  onChange: (variant: Variant) => void;
  onRemove: () => void;
}

const VariantForm: React.FC<VariantFormProps> = ({
  variant,
  index,
  isSingleVariant,
  isLoading,
  productId,
  axiosInstance,
  onChange,
  onRemove,
}) => {
  const [priceInput, setPriceInput] = useState<string>("");
  const [stockInput, setStockInput] = useState<string>("");

  useEffect(() => {
    setPriceInput(variant.price === 0 ? "" : variant.price.toString());
    setStockInput(variant.stock === 0 ? "" : variant.stock.toString());
  }, [variant.price, variant.stock]);

  const parseNumber = (value: string): number => {
    const cleanedValue = value
      .replace(/[^0-9.]/g, "")
      .replace(/(\..*)\./g, "$1");
    if (cleanedValue === "" || isNaN(parseFloat(cleanedValue))) {
      return 0;
    }
    return parseFloat(parseFloat(cleanedValue).toFixed(2));
  };

  const isValidInput = (value: string): boolean => {
    return /^[0-9]*\.?[0-9]*$/.test(value);
  };

  const handleChange = (field: keyof Variant, value: string) => {
    if (field === "price") {
      if (isValidInput(value)) {
        setPriceInput(value);
        const parsedValue = parseNumber(value);
        onChange({ ...variant, price: parsedValue });
      }
    } else if (field === "stock") {
      if (isValidInput(value)) {
        setStockInput(value);
        const parsedValue = parseNumber(value);
        onChange({ ...variant, stock: parsedValue });
      }
    } else {
      onChange({ ...variant, [field]: value });
    }
  };

  const handleBlur = (field: "price" | "stock") => {
    const inputValue = field === "price" ? priceInput : stockInput;
    const parsedValue = parseNumber(inputValue);
    const displayValue = inputValue
      .replace(/[^0-9.]/g, "")
      .replace(/(\..*)\./g, "$1");
    if (field === "price") {
      onChange({ ...variant, price: parsedValue });
      setPriceInput(displayValue === "" ? "" : displayValue);
    } else {
      onChange({ ...variant, stock: parsedValue });
      setStockInput(displayValue === "" ? "" : displayValue);
    }
  };

  const handleAttributesChange = (attributes: VariantAttribute[]) => {
    onChange({ ...variant, attributes });
  };

  const handleRemove = async () => {
    if (productId && variant.id) {
      try {
        const response = await axiosInstance.delete(
          `/product/${productId}/variant/${variant.id}`
        );
        if (response.data.code === 200) {
          toast.success("Variant removed successfully");
          onRemove();
        } else {
          toast.error("Failed to remove variant");
        }
      } catch (err: any) {
        toast.error(err.message || "Error removing variant");
      }
    } else {
      onRemove();
    }
  };

  return (
    <AccordionItem value={`variant-${index}`}>
      <Card className="mt-4 px-4 py-2 text-lg hover:cursor-pointer">
        <AccordionTrigger>
          {variant.name || `Variant ${index + 1}`}
        </AccordionTrigger>
        <AccordionContent>
          <CardContent className="space-y-4">
            <div>
              <Label className="mb-2" htmlFor={`variant-name-${index}`}>
                Name
              </Label>
              <Input
                id={`variant-name-${index}`}
                value={variant.name}
                onChange={(e) => handleChange("name", e.target.value)}
                placeholder="Enter variant name"
                disabled={isLoading}
              />
            </div>
            <div>
              <Label className="mb-2" htmlFor={`variant-sku-${index}`}>
                SKU
              </Label>
              <Input
                id={`variant-sku-${index}`}
                value={variant.sku}
                onChange={(e) => handleChange("sku", e.target.value)}
                placeholder="Enter SKU"
                disabled={isLoading}
              />
            </div>
            <div>
              <Label className="mb-2" htmlFor={`variant-price-${index}`}>
                Price
              </Label>
              <Input
                id={`variant-price-${index}`}
                type="text"
                value={priceInput}
                onChange={(e) => handleChange("price", e.target.value)}
                onBlur={() => handleBlur("price")}
                placeholder="Enter price (e.g., 99.99)"
                disabled={isLoading}
              />
            </div>
            <div>
              <Label className="mb-2" htmlFor={`variant-stock-${index}`}>
                Stock
              </Label>
              <Input
                id={`variant-stock-${index}`}
                type="text"
                value={stockInput}
                onChange={(e) => handleChange("stock", e.target.value)}
                onBlur={() => handleBlur("stock")}
                placeholder="Enter stock (e.g., 100.50)"
                disabled={isLoading}
              />
            </div>
            <AttributeForm
              attributes={variant.attributes}
              isLoading={isLoading}
              onAttributesChange={handleAttributesChange}
            />
            <Button
              type="button"
              variant="destructive"
              size="sm"
              onClick={handleRemove}
              disabled={isSingleVariant || isLoading}
            >
              Remove Variant
            </Button>
          </CardContent>
        </AccordionContent>
      </Card>
    </AccordionItem>
  );
};

export default VariantForm;
