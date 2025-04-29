// src/pages/admin/ProductPage.tsx

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
  DialogTrigger,
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  Input,
  Label,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
  Textarea,
} from "@/components/ui";
import { MoreVertical, Pencil, Trash } from "lucide-react";

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

let productIdCounter = 3;
let variantIdCounter = 1;

function ProductPage() {
  const [products, setProducts] = useState<Product[]>([
    {
      id: 1,
      name: "Wireless Mouse",
      category: "Mice",
      description: "A nice wireless mouse",
      variants: [],
    },
    {
      id: 2,
      name: "Mechanical Keyboard",
      category: "Keyboards",
      description: "Tactile mechanical keyboard",
      variants: [],
    },
  ]);

  const [openProductDialog, setOpenProductDialog] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);

  const [newProductName, setNewProductName] = useState("");
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);

  const [newProductCategory, setNewProductCategory] = useState("");
  const [newProductDescription, setNewProductDescription] = useState("");
  const [variants, setVariants] = useState<Variant[]>([]);

  const [openVariantDialog, setOpenVariantDialog] = useState(false);
  const [editingVariant, setEditingVariant] = useState<Variant | null>(null);

  const [variantName, setVariantName] = useState("");
  const [variantSku, setVariantSku] = useState("");
  const [variantPrice, setVariantPrice] = useState("");
  const [variantStock, setVariantStock] = useState("");

  const resetProductForm = () => {
    setNewProductName("");
    setNewProductCategory("");
    setNewProductDescription("");
    setVariants([]);
  };

  const openAddProduct = () => {
    resetProductForm();
    setEditingProduct(null);
    setOpenProductDialog(true);
  };

  const handleEdit = (product: Product) => {
    setSelectedProduct(product);
    setOpenProductDialog(true);
  };

  const handleDelete = (productId: number) => {
    console.log("Delete product with id:", productId);
  };

  const saveProduct = () => {
    if (!editingProduct) {
      setProducts((prev) => [
        ...prev,
        {
          id: productIdCounter++,
          name: newProductName,
          category: newProductCategory,
          description: newProductDescription,
          variants: variants,
        },
      ]);
    } else {
      setProducts((prev) =>
        prev.map((p) =>
          p.id === editingProduct.id
            ? {
                ...p,
                name: newProductName,
                category: newProductCategory,
                description: newProductDescription,
                variants,
              }
            : p
        )
      );
    }
    setOpenProductDialog(false);
  };

  const startEditProduct = (product: Product) => {
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
                id: v.id,
                name: variantName,
                sku: variantSku,
                price: parseFloat(variantPrice),
                stock: parseInt(variantStock, 10),
              }
            : v
        )
      );
    } else {
      setVariants((prev) => [
        ...prev,
        {
          id: variantIdCounter++,
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

  const deleteProduct = (id: number) => {
    setProducts((prev) => prev.filter((p) => p.id !== id));
  };

  return (
    <div className="p-6 space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Products</h1>
        <Button onClick={openAddProduct}>Add Product</Button>
      </div>

      <Card className="w-full">
        <CardHeader>
          <CardTitle>Product List</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Name</TableHead>
                <TableHead>Category</TableHead>
                <TableHead>Description</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {products.map((product) => (
                <TableRow key={product.id}>
                  <TableCell>{product.name}</TableCell>
                  <TableCell>{product.category}</TableCell>
                  <TableCell>{product.description}</TableCell>
                  <TableCell className="text-right">
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="ghost" className="h-8 w-8 p-0">
                          <MoreVertical className="h-4 w-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem onClick={() => handleEdit(product)}>
                          Edit
                        </DropdownMenuItem>
                        <DropdownMenuItem
                          onClick={() => handleDelete(product.id)}
                        >
                          Delete
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      {/* Product Dialog */}
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
                  placeholder="Product Name"
                />
              </div>
              <div>
                <Label>Category</Label>
                <Input
                  value={newProductCategory}
                  onChange={(e) => setNewProductCategory(e.target.value)}
                  placeholder="Category"
                />
              </div>
            </div>
            <div>
              <Label>Description</Label>
              <Textarea
                value={newProductDescription}
                onChange={(e) => setNewProductDescription(e.target.value)}
                placeholder="Description"
              />
            </div>

            {/* Variant Management */}
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
              onClick={saveProduct}
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
