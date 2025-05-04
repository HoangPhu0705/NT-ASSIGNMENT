import React from "react";
import {
  Button,
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  Input,
  Label,
} from "@/components/ui";
import { Loader2 } from "lucide-react";

interface Product {
  id: string;
  name: string;
}

interface ProductDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  editingProduct: Product | null;
  newProductName: string;
  setNewProductName: (value: string) => void;
  onSave: () => void;
  isLoading: boolean;
}

const ProductDialog: React.FC<ProductDialogProps> = ({
  open,
  onOpenChange,
  editingProduct,
  newProductName,
  setNewProductName,
  onSave,
  isLoading,
}) => {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {editingProduct ? "Edit Product" : "Add Product"}
          </DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          <div>
            <Label className="mb-2">Name</Label>
            <Input
              value={newProductName}
              onChange={(e) => setNewProductName(e.target.value)}
              placeholder="Product Name"
              disabled={isLoading}
            />
          </div>
          <Button
            className="w-full"
            onClick={onSave}
            disabled={isLoading || !newProductName.trim()}
          >
            {isLoading ? (
              <>
                <Loader2 className="animate-spin mr-2 h-4 w-4" />
                Please wait
              </>
            ) : editingProduct ? (
              "Save Changes"
            ) : (
              "Add Product"
            )}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ProductDialog;
