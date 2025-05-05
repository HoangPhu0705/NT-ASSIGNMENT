/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-unused-vars */
import React, { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast/headless";
import useAxios from "@/hooks/useAxios";
import ProductFilters from "@/components/common/product/product-filter";
import ProductTable from "@/components/common/product/product-table";
import { Button } from "@/components/ui";
import { updateProductImage, uploadProductImage } from "@/lib/productImages";

interface VariantAttribute {
  name: string;
  value: string;
}

interface Variant {
  id: string;
  name: string;
  sku: string;
  price: number;
  stock: number;
  attributes: VariantAttribute[];
}

interface ProductImage {
  id: string;
  imageUrl: string;
  isPrimary: boolean;
}

interface Product {
  id: string;
  name: string;
  price: number;
  images: ProductImage[];
  mainImageUrl: string;
  categoryId: string;
  categoryName: string;
  variants: Variant[];
}

interface Category {
  id: string;
  name: string;
  description: string;
  imageUrl: string;
}

function ProductPage() {
  const { axiosInstance, isLoading } = useAxios();
  const navigate = useNavigate();
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [categoryFilter, setCategoryFilter] = useState("");
  const [sortOption, setSortOption] = useState("");
  const [searchKeyword, setSearchKeyword] = useState("");

  // Fetch products
  const fetchProducts = useCallback(async () => {
    try {
      const response = await axiosInstance.get("/product");
      if (response.data.code === 200) {
        console.log("Fetched products:", response.data.data);
        setProducts(response.data.data);
      } else {
        toast.error("Error fetching products");
      }
    } catch (err) {
      toast.error("Error fetching products");
    }
  }, [axiosInstance]);

  // Fetch categories
  const fetchCategories = useCallback(async () => {
    try {
      const response = await axiosInstance.get("/category");
      if (response.data.code === 200) {
        setCategories(response.data.data);
      } else {
        toast.error("Error fetching categories");
      }
    } catch (err) {
      toast.error("Error fetching categories");
    }
  }, [axiosInstance]);

  useEffect(() => {
    fetchProducts();
    fetchCategories();
  }, [fetchProducts, fetchCategories]);

  const handleAddProduct = () => {
    navigate("/product/add");
  };

  const handleEditProduct = (product: Product) => {
    navigate(`/product/edit/${product.id}`);
  };

  const handleDeleteProduct = async (id: string) => {
    try {
      await axiosInstance.delete(`/product/${id}`);
      toast.success("Product deleted successfully");
      setProducts((prev) => prev.filter((p) => p.id !== id));
    } catch (err: any) {
      toast.error(err.message || "Error deleting product");
    }
  };

  const handleChangeImage = async (
    id: string,
    file: File,
    existingImageUrl?: string
  ) => {
    try {
      let newImageUrl: string;
      // Find the product to get its images array
      const product = products.find((p) => p.id === id);
      if (!product) {
        throw new Error("Product not found");
      }

      // Find the current primary image
      const primaryImage = product.images.find((img) => img.isPrimary);
      const isValidSupabaseUrl =
        primaryImage &&
        primaryImage.imageUrl &&
        primaryImage.imageUrl.includes("nt-bucket") &&
        primaryImage.imageUrl.includes("product-images/");

      if (isValidSupabaseUrl && primaryImage) {
        const filePath = primaryImage.imageUrl.split("/").slice(-2).join("/");
        if (!filePath || !filePath.includes("product-images/")) {
          console.error("Invalid file path extracted:", {
            imageUrl: primaryImage.imageUrl,
            filePath,
          });
          throw new Error("Invalid existing image URL");
        }
        console.log("Updating existing image:", { id, filePath });
        newImageUrl = await updateProductImage(file, filePath);
      } else {
        console.log("Uploading new image for product:", {
          id,
          existingImageUrl,
        });
        newImageUrl = await uploadProductImage(file);
      }

      // Construct the updated images array
      const updatedImages = product.images.map((img) => ({
        id: img.id,
        imageUrl: img.isPrimary ? newImageUrl : img.imageUrl,
        isMain: img.isPrimary,
      }));

      // If no primary image exists, add a new one
      if (!primaryImage) {
        updatedImages.push({
          id: crypto.randomUUID(),
          imageUrl: newImageUrl,
          isMain: true,
        });
      }

      console.log("Sending PATCH request with images:", updatedImages);
      await axiosInstance.patch(`/product/${id}`, {
        images: updatedImages,
      });

      toast.success("Image updated successfully");
      setProducts((prev) =>
        prev.map((p) =>
          p.id === id
            ? {
                ...p,
                images: updatedImages.map((img) => ({
                  id: img.id,
                  imageUrl: img.imageUrl,
                  isPrimary: img.isMain,
                })),
                mainImageUrl: newImageUrl,
              }
            : p
        )
      );
    } catch (err: any) {
      console.error("Error in handleChangeImage:", {
        error: err.message,
        existingImageUrl,
      });
      toast.error(err.message || "Error updating image");
    }
  };

  // Filter and sort products
  const filteredProducts = products
    .filter((product) =>
      product.name.toLowerCase().includes(searchKeyword.toLowerCase())
    )
    .filter((product) =>
      categoryFilter && categoryFilter !== "all"
        ? product.categoryName === categoryFilter
        : true
    )
    .sort((a, b) => {
      if (sortOption === "name-asc") return a.name.localeCompare(b.name);
      if (sortOption === "name-desc") return b.name.localeCompare(a.name);
      return 0;
    });

  return (
    <div className="p-6 space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Products</h1>
        <Button onClick={handleAddProduct} disabled={isLoading}>
          Add Product
        </Button>
      </div>
      <ProductFilters
        searchKeyword={searchKeyword}
        setSearchKeyword={setSearchKeyword}
        categoryFilter={categoryFilter}
        setCategoryFilter={setCategoryFilter}
        sortOption={sortOption}
        setSortOption={setSortOption}
        categories={categories.map((c) => c.name)}
        onAddProduct={handleAddProduct}
      />
      <ProductTable
        products={filteredProducts}
        onEdit={handleEditProduct}
        onDelete={handleDeleteProduct}
        onChangeImage={handleChangeImage}
      />
    </div>
  );
}

export default ProductPage;
