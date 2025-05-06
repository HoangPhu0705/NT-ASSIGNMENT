import React from "react";
import { Button, Accordion } from "@/components/ui";
import VariantForm from "./variant-form";
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

interface VariantListProps {
  variants: Variant[];
  error?: string;
  isLoading: boolean;
  productId?: string;
  axiosInstance: AxiosInstance;
  onVariantsChange: (variants: Variant[]) => void;
  onAddVariant: () => void;
}

const VariantList: React.FC<VariantListProps> = ({
  variants,
  error,
  isLoading,
  productId,
  axiosInstance,
  onVariantsChange,
  onAddVariant,
}) => {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-xl font-semibold">Variants</h2>
        <Button type="button" onClick={onAddVariant} disabled={isLoading}>
          Add Variant
        </Button>
      </div>
      {error && <p className="text-red-500 text-sm">{error}</p>}
      <Accordion type="multiple">
        {variants.map((variant, index) => (
          <VariantForm
            key={index}
            variant={variant}
            index={index}
            isSingleVariant={variants.length === 1}
            isLoading={isLoading}
            productId={productId}
            axiosInstance={axiosInstance}
            onChange={(updatedVariant) => {
              const newVariants = [...variants];
              newVariants[index] = updatedVariant;
              onVariantsChange(newVariants);
            }}
            onRemove={() => {
              onVariantsChange(variants.filter((_, i) => i !== index));
            }}
          />
        ))}
      </Accordion>
    </div>
  );
};

export default VariantList;
