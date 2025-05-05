import React, { useState } from "react";
import {
  Card,
  CardHeader,
  CardTitle,
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
  const [priceInput, setPriceInput] = useState<string>();

  const validateAndParsePrice = (value: string): number => {
    if (value === "") return 0;
    const regex = /^\d+(\.\d{1,2})?$/; // Matches 99, 99.9, 99.99
    if (regex.test(value)) {
      return parseFloat(value);
    }
    return NaN; // Invalid input
  };

  const handleChange = (field: keyof Variant, value: any) => {
    if (field === "price") {
      const permissiveRegex = /^\d*(\.\d{0,2})?$/; // Allows 99, 99., 99.9, 99.99
      if (value === "" || permissiveRegex.test(value)) {
        setPriceInput(value);
        const parsedValue = validateAndParsePrice(value);
        if (!isNaN(parsedValue)) {
          onChange({ ...variant, [field]: parsedValue });
        }
      }
    } else if (field === "stock") {
      const parsedValue = Number(value);
      if (
        !isNaN(parsedValue) &&
        parsedValue >= 0 &&
        Number.isInteger(parsedValue)
      ) {
        onChange({ ...variant, [field]: parsedValue });
      }
    } else {
      onChange({ ...variant, [field]: value });
    }
  };

  const handlePriceBlur = () => {
    const parsedValue = validateAndParsePrice(priceInput);
    const finalValue = isNaN(parsedValue) ? 0 : parsedValue;
    onChange({ ...variant, price: finalValue });
    setPriceInput(finalValue === 0 ? "" : finalValue.toString());
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
          onRemove(); // Update local state
        } else {
          toast.error("Failed to remove variant");
        }
      } catch (err: any) {
        toast.error(err.message || "Error removing variant");
      }
    } else {
      // In add mode or if variant.id is missing, remove locally
      onRemove();
    }
  };

  return (
    <AccordionItem value={`variant-${index}`}>
      <Card className="mt-4 px-4 py-2 text-lg hover:cursor-pointer">
        <AccordionTrigger>{variant.name}</AccordionTrigger>
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
                value={variant.price.toString()}
                onChange={(e) => handleChange("price", e.target.value)}
                onBlur={handlePriceBlur}
                placeholder="Enter price (e.g., 99.99)"
                pattern="^\d+(\.\d{1,2})?$"
                disabled={isLoading}
              />
            </div>
            <div>
              <Label className="mb-2" htmlFor={`variant-stock-${index}`}>
                Stock
              </Label>
              <Input
                id={`variant-stock-${index}`}
                type="number"
                value={variant.stock}
                onChange={(e) => handleChange("stock", e.target.value)}
                placeholder="Enter stock (e.g., 100)"
                min="0"
                disabled={isLoading}
              />
            </div>
            <AttributeForm
              attributes={variant.attributes}
              isLoading={isLoading}
              onAttributesChange={handleAttributesChange}
            />
            <Button
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
