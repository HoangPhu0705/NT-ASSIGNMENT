import React from "react";
import {
  Button,
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  Input,
} from "@/components/ui";

interface Variant {
  id: number;
  name: string;
  sku: string;
  price: number;
  stock: number;
}

interface VariantDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  editingVariant: Variant | null;
  variantName: string;
  setVariantName: (value: string) => void;
  variantSku: string;
  setVariantSku: (value: string) => void;
  variantPrice: string;
  setVariantPrice: (value: string) => void;
  variantStock: string;
  setVariantStock: (value: string) => void;
  onSave: () => void;
}

const VariantDialog: React.FC<VariantDialogProps> = ({
  open,
  onOpenChange,
  editingVariant,
  variantName,
  setVariantName,
  variantSku,
  setVariantSku,
  variantPrice,
  setVariantPrice,
  variantStock,
  setVariantStock,
  onSave,
}) => {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {editingVariant ? "Edit Variant" : "Add Variant"}
          </DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          <Input
            value={variantName}
            onChange={(e) => setVariantName(e.target.value)}
            placeholder="Variant Name"
          />
          <Input
            value={variantSku}
            onChange={(e) => setVariantSku(e.target.value)}
            placeholder="SKU"
          />
          <Input
            value={variantPrice}
            onChange={(e) => setVariantPrice(e.target.value)}
            placeholder="Price"
            type="number"
          />
          <Input
            value={variantStock}
            onChange={(e) => setVariantStock(e.target.value)}
            placeholder="Stock"
            type="number"
          />
          <Button className="w-full" onClick={onSave}>
            {editingVariant ? "Save Changes" : "Add Variant"}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default VariantDialog;
