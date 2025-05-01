import React, { useState } from "react";
import {
  Button,
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  Input,
  Label,
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
  Textarea,
} from "@/components/ui";
import { Pencil, Trash } from "lucide-react";

interface Variant {
  id: number;
  name: string;
  sku: string;
  price: number;
  stock: number;
}

interface Product {
  id: number;
  name: string;
  category: string;
  description: string;
  variants: Variant[];
}

function ProductPage() {
  // UI-only states
  const [openProductDialog, setOpenProductDialog] = useState(false);
  const [openVariantDialog, setOpenVariantDialog] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [editingVariant, setEditingVariant] = useState<Variant | null>(null);

  const [newProductName, setNewProductName] = useState("");
  const [newProductCategory, setNewProductCategory] = useState("");
  const [newProductDescription, setNewProductDescription] = useState("");
  const [variants, setVariants] = useState<Variant[]>([]);

  const [variantName, setVariantName] = useState("");
  const [variantSku, setVariantSku] = useState("");
  const [variantPrice, setVariantPrice] = useState("");
  const [variantStock, setVariantStock] = useState("");

  // Filters
  const [categoryFilter, setCategoryFilter] = useState("");
  const [sortOption, setSortOption] = useState("");
  const [searchKeyword, setSearchKeyword] = useState("");

  const handleAddProduct = () => {
    setEditingProduct(null);
    setNewProductName("");
    setNewProductCategory("");
    setNewProductDescription("");
    setVariants([]);
    setOpenProductDialog(true);
  };

  const handleEditProduct = (product: Product) => {
    setEditingProduct(product);
    setNewProductName(product.name);
    setNewProductCategory(product.category);
    setNewProductDescription(product.description);
    setVariants(product.variants);
    setOpenProductDialog(true);
  };

  const startAddVariant = () => {
    setEditingVariant(null);
    setVariantName("");
    setVariantSku("");
    setVariantPrice("");
    setVariantStock("");
    setOpenVariantDialog(true);
  };

  const startEditVariant = (variant: Variant) => {
    setEditingVariant(variant);
    setVariantName(variant.name);
    setVariantSku(variant.sku);
    setVariantPrice(variant.price.toString());
    setVariantStock(variant.stock.toString());
    setOpenVariantDialog(true);
  };

  const saveVariant = () => {
    if (!variantName || !variantSku || !variantPrice || !variantStock) return;
    if (editingVariant) {
      setVariants((prev) =>
        prev.map((v) =>
          v.id === editingVariant.id
            ? {
                ...v,
                name: variantName,
                sku: variantSku,
                price: parseFloat(variantPrice),
                stock: parseInt(variantStock, 10),
              }
            : v
        )
      );
    } else {
      const newId = Date.now();
      setVariants((prev) => [
        ...prev,
        {
          id: newId,
          name: variantName,
          sku: variantSku,
          price: parseFloat(variantPrice),
          stock: parseInt(variantStock, 10),
        },
      ]);
    }
    setOpenVariantDialog(false);
  };

  const deleteVariant = (id: number) => {
    setVariants((prev) => prev.filter((v) => v.id !== id));
  };

  return (
    <div className="p-6 space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <h1 className="text-2xl font-bold">Products</h1>
        <div className="flex flex-col sm:flex-row gap-4 w-full sm:w-auto">
          <Input
            placeholder="Search products..."
            value={searchKeyword}
            onChange={(e) => setSearchKeyword(e.target.value)}
          />
          <Select value={categoryFilter} onValueChange={setCategoryFilter}>
            <SelectTrigger className="w-40">
              <SelectValue placeholder="Filter Category" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="mice">Mice</SelectItem>
              <SelectItem value="keyboards">Keyboards</SelectItem>
              <SelectItem value="headsets">Headsets</SelectItem>
            </SelectContent>
          </Select>
          <Select value={sortOption} onValueChange={setSortOption}>
            <SelectTrigger className="w-40">
              <SelectValue placeholder="Sort By" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="name-asc">Name A-Z</SelectItem>
              <SelectItem value="name-desc">Name Z-A</SelectItem>
              <SelectItem value="date-newest">Newest</SelectItem>
              <SelectItem value="date-oldest">Oldest</SelectItem>
            </SelectContent>
          </Select>
          <Button onClick={handleAddProduct}>Add Product</Button>
        </div>
      </div>

      {/* Placeholder for Product Table (waiting for API connection) */}
      <Card className="w-full">
        <CardHeader>
          <CardTitle>Product List</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="text-sm text-muted-foreground text-center py-8">
            Products will be listed here after API integration.
          </div>
        </CardContent>
      </Card>

      {/* Add/Edit Product Dialog */}
      <Dialog open={openProductDialog} onOpenChange={setOpenProductDialog}>
        <DialogContent className="max-w-3xl">
          <DialogHeader>
            <DialogTitle>
              {editingProduct ? "Edit Product" : "Add Product"}
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label>Name</Label>
                <Input
                  value={newProductName}
                  onChange={(e) => setNewProductName(e.target.value)}
                />
              </div>
              <div>
                <Label>Category</Label>
                <Input
                  value={newProductCategory}
                  onChange={(e) => setNewProductCategory(e.target.value)}
                />
              </div>
            </div>
            <div>
              <Label>Description</Label>
              <Textarea
                value={newProductDescription}
                onChange={(e) => setNewProductDescription(e.target.value)}
              />
            </div>

            <div>
              <div className="flex items-center justify-between mb-2">
                <Label>Variants</Label>
                <Button size="sm" onClick={startAddVariant}>
                  Add Variant
                </Button>
              </div>
              {variants.length > 0 ? (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Name</TableHead>
                      <TableHead>SKU</TableHead>
                      <TableHead>Price</TableHead>
                      <TableHead>Stock</TableHead>
                      <TableHead className="text-right">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {variants.map((variant) => (
                      <TableRow key={variant.id}>
                        <TableCell>{variant.name}</TableCell>
                        <TableCell>{variant.sku}</TableCell>
                        <TableCell>${variant.price.toFixed(2)}</TableCell>
                        <TableCell>{variant.stock}</TableCell>
                        <TableCell className="text-right space-x-2">
                          <Button
                            size="icon"
                            variant="ghost"
                            onClick={() => startEditVariant(variant)}
                          >
                            <Pencil className="w-4 h-4" />
                          </Button>
                          <Button
                            size="icon"
                            variant="ghost"
                            onClick={() => deleteVariant(variant.id)}
                          >
                            <Trash className="w-4 h-4" />
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              ) : (
                <p className="text-sm text-muted-foreground">
                  No variants added yet.
                </p>
              )}
            </div>

            <Button
              className="w-full"
              disabled={variants.length === 0}
              onClick={() => setOpenProductDialog(false)}
            >
              Save Product
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Variant Dialog */}
      <Dialog open={openVariantDialog} onOpenChange={setOpenVariantDialog}>
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
            <Button className="w-full" onClick={saveVariant}>
              {editingVariant ? "Save Changes" : "Add Variant"}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}

export default ProductPage;
