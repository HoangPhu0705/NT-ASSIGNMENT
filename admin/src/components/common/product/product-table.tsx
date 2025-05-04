import React, { useState } from "react";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
  Button,
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  Input,
  Label,
} from "@/components/ui";
import { Pencil, Trash, Loader2 } from "lucide-react";
import useAxios from "@/hooks/useAxios";

interface Variant {
  id: string;
  name: string;
  sku: string;
  price: number;
  stock: number;
  attributes: { name: string; value: string }[];
}

interface Product {
  id: string;
  name: string;
  price: number;
  images: { id: string; imageUrl: string; isPrimary: boolean }[];
  mainImageUrl: string;
  categoryId: string;
  categoryName: string;
  variants: Variant[];
}

interface ChangeImageDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  product: Product;
  onSave: (id: string, file: File, existingImageUrl?: string) => void;
}

const ChangeImageDialog: React.FC<ChangeImageDialogProps> = ({
  open,
  onOpenChange,
  product,
  onSave,
}) => {
  const { isLoading } = useAxios();
  const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setSelectedFile(e.target.files[0]);
    }
  };

  const handleSave = () => {
    if (selectedFile) {
      onSave(product.id, selectedFile, product.mainImageUrl);
      setSelectedFile(null);
      onOpenChange(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Change Product Image</DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          <div>
            <Label className="mb-2">Upload Image</Label>
            <Input
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              disabled={isLoading}
            />
            {product.mainImageUrl && (
              <div className="mt-2">
                <p className="text-sm text-muted-foreground">Current Image:</p>
                <img
                  src={product.mainImageUrl}
                  alt="Current product"
                  className="w-24 h-24 object-cover rounded mt-1"
                />
              </div>
            )}
          </div>
          <div className="flex flex-col gap-2">
            <Button
              className="w-full"
              onClick={handleSave}
              disabled={isLoading || !selectedFile}
            >
              {isLoading ? (
                <>
                  <Loader2 className="animate-spin mr-2 h-4 w-4" />
                  Please wait
                </>
              ) : (
                "Save"
              )}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};

interface ProductTableProps {
  products: Product[];
  onEdit: (product: Product) => void;
  onDelete: (id: string) => void;
  onChangeImage: (id: string, file: File, existingImageUrl?: string) => void;
}

const ProductTable: React.FC<ProductTableProps> = ({
  products,
  onEdit,
  onDelete,
  onChangeImage,
}) => {
  const [selectedVariants, setSelectedVariants] = useState<{
    [key: string]: string;
  }>({});
  const [openImageDialog, setOpenImageDialog] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);

  const handleVariantChange = (productId: string, variantId: string) => {
    setSelectedVariants((prev) => ({ ...prev, [productId]: variantId }));
  };

  const handleOpenImageDialog = (product: Product) => {
    setSelectedProduct(product);
    setOpenImageDialog(true);
  };

  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>Product List</CardTitle>
      </CardHeader>
      <CardContent>
        {products.length === 0 ? (
          <div className="text-sm text-muted-foreground text-center py-8">
            No products found.
          </div>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Name</TableHead>
                <TableHead>Variant</TableHead>
                <TableHead>Price</TableHead>
                <TableHead>Image</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {products.map((product) => {
                const selectedVariantId =
                  selectedVariants[product.id] || product.variants[0]?.id;
                const selectedVariant = product.variants.find(
                  (v) => v.id === selectedVariantId
                );
                return (
                  <TableRow key={product.id}>
                    <TableCell>{product.name}</TableCell>
                    <TableCell>
                      {product.variants.length > 0 ? (
                        <Select
                          value={selectedVariantId}
                          onValueChange={(value) =>
                            handleVariantChange(product.id, value)
                          }
                        >
                          <SelectTrigger className="w-48">
                            <SelectValue placeholder="Select variant" />
                          </SelectTrigger>
                          <SelectContent>
                            {product.variants.map((variant) => (
                              <SelectItem key={variant.id} value={variant.id}>
                                {variant.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      ) : (
                        "No variants"
                      )}
                    </TableCell>
                    <TableCell>
                      {selectedVariant
                        ? `$${selectedVariant.price.toFixed(2)}`
                        : "No variants"}
                    </TableCell>
                    <TableCell>
                      <TooltipProvider>
                        <Tooltip>
                          <TooltipTrigger asChild>
                            <div
                              className="cursor-pointer"
                              onClick={() => handleOpenImageDialog(product)}
                            >
                              {product.mainImageUrl ? (
                                <img
                                  src={product.mainImageUrl}
                                  alt={product.name}
                                  className="w-16 h-16 object-cover rounded"
                                />
                              ) : (
                                <span className="text-sm text-muted-foreground">
                                  No image
                                </span>
                              )}
                            </div>
                          </TooltipTrigger>
                          <TooltipContent>
                            <p>Change Image</p>
                          </TooltipContent>
                        </Tooltip>
                      </TooltipProvider>
                    </TableCell>
                    <TableCell className="text-right space-x-2">
                      <Button
                        size="icon"
                        variant="ghost"
                        onClick={() => onEdit(product)}
                      >
                        <Pencil className="w-4 h-4" />
                      </Button>
                      <Button
                        size="icon"
                        variant="ghost"
                        onClick={() => onDelete(product.id)}
                      >
                        <Trash className="w-4 h-4" />
                      </Button>
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        )}
      </CardContent>
      {selectedProduct && (
        <ChangeImageDialog
          open={openImageDialog}
          onOpenChange={setOpenImageDialog}
          product={selectedProduct}
          onSave={onChangeImage}
        />
      )}
    </Card>
  );
};

export default ProductTable;
