/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import toast from "react-hot-toast";
import useAxios from "@/hooks/useAxios";
import { uploadProductImage } from "@/lib/productImages";
import { Button } from "@/components/ui";
import ProductDetails from "@/components/common/product/product-details";
import ImageUploader from "@/components/common/product/image-uploader";
import VariantList from "@/components/common/product/variant-list";
import { Loader2 } from "lucide-react";

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

interface ProductImage {
  id?: string;
  imageUrl: string;
  isPrimary: boolean;
  isDeleted?: boolean;
  file?: File;
}

interface Product {
  id?: string;
  name: string;
  description: string;
  categoryId: string;
  isParent: boolean;
  images: ProductImage[];
  variants: Variant[];
}

interface Category {
  id: string;
  name: string;
}

const ProductForm: React.FC = () => {
  const { axiosInstance, isLoading } = useAxios();
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEditMode = !!id;

  const [categories, setCategories] = useState<Category[]>([]);
  const [product, setProduct] = useState<Product>({
    name: "",
    description: "",
    categoryId: "",
    isParent: true,
    images: [],
    variants: [{ name: "", sku: "", price: 0, stock: 0, attributes: [] }],
  });
  const [errors, setErrors] = useState<{
    name?: string;
    categoryId?: string;
    variants?: string;
    images?: string;
  }>({});
  const [uploading, setUploading] = useState(false);

  // Fetch categories
  useEffect(() => {
    const fetchCategories = async () => {
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
    };
    fetchCategories();
  }, [axiosInstance]);

  // Fetch product data for edit mode
  useEffect(() => {
    if (isEditMode) {
      const fetchProduct = async () => {
        try {
          const response = await axiosInstance.get(`/product/${id}`);
          if (response.data.code === 200) {
            const productData = response.data.data;
            setProduct({
              name: productData.name,
              description: productData.description || "",
              categoryId: productData.categoryId,
              isParent: productData.isParent ?? true,
              images: productData.images.map((img: any) => ({
                id: img.id,
                imageUrl: img.imageUrl,
                isPrimary: img.isPrimary,
                isDeleted: false,
              })),
              variants: productData.variants.map((variant: any) => ({
                id: variant.id,
                name: variant.name,
                sku: variant.sku,
                price: variant.price,
                stock: variant.stock,
                attributes: variant.attributes,
              })),
            });
          } else {
            toast.error("Error fetching product");
          }
        } catch (err) {
          toast.error("Error fetching product");
        }
      };
      fetchProduct();
    }
  }, [axiosInstance, id, isEditMode]);

  const validateForm = (): boolean => {
    const newErrors: typeof errors = {};
    if (!product.name.trim()) {
      newErrors.name = "Product name is required";
    }
    if (!product.categoryId) {
      newErrors.categoryId = "Category is required";
    }
    if (product.variants.length === 0) {
      newErrors.variants = "At least one variant is required";
    } else {
      product.variants.forEach((variant, index) => {
        if (!variant.name.trim()) {
          newErrors.variants = `Variant ${index + 1}: Name is required`;
        }
        if (!variant.sku.trim()) {
          newErrors.variants = `Variant ${index + 1}: SKU is required`;
        }
        if (variant.price <= 0) {
          newErrors.variants = `Variant ${
            index + 1
          }: Price must be greater than 0`;
        }
        if (variant.stock < 0) {
          newErrors.variants = `Variant ${index + 1}: Stock cannot be negative`;
        }
      });
    }
    const activeImages = product.images.filter((img) => !img.isDeleted);
    if (activeImages.length > 0 && !activeImages.some((img) => img.isPrimary)) {
      newErrors.images = "One image must be set as primary";
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleImageUpload = async (file: File): Promise<string> => {
    const validTypes = ["image/jpeg", "image/png", "image/gif"];
    if (!validTypes.includes(file.type)) {
      throw new Error("Only JPEG, PNG, or GIF images are allowed");
    }
    if (file.size > 5 * 1024 * 1024) {
      throw new Error("Image size must be less than 5MB");
    }
    return await uploadProductImage(file);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) {
      toast.error("Please fix the form errors");
      return;
    }

    setUploading(true);
    try {
      const uploadedImages = await Promise.all(
        product.images.map(async (img) => ({
          id: img.id,
          imageUrl: img.file ? await handleImageUpload(img.file) : img.imageUrl,
          isMain: img.isPrimary,
          isDeleted: img.isDeleted || false,
        }))
      );

      const requestBody = {
        name: product.name,
        description: product.description,
        categoryId: product.categoryId,
        isParent: product.isParent,
        images: uploadedImages,
        variants: product.variants,
      };

      console.info("Request Body:", JSON.stringify(requestBody, null, 2));

      if (isEditMode) {
        await axiosInstance.patch(`/product/${id}`, requestBody);
        toast.success("Product updated successfully");
        window.location.reload();
      } else {
        await axiosInstance.post("/product", requestBody);
        toast.success("Product created successfully");
      }
    } catch (err: any) {
      toast.error(err.message || "Error saving product");
    } finally {
      setUploading(false);
    }
  };

  const handleAddVariant = () => {
    setProduct((prev) => ({
      ...prev,
      variants: [
        ...prev.variants,
        { name: "", sku: "", price: 0, stock: 0, attributes: [] },
      ],
    }));
  };

  return (
    <div className="p-6 mx-auto">
      <h1 className="text-2xl font-bold mb-6">
        {isEditMode ? "Edit Product" : "Add Product"}
      </h1>
      <form onSubmit={handleSubmit} className="space-y-6">
        <ProductDetails
          product={product}
          categories={categories}
          errors={errors}
          onChange={(field: any, value: any) =>
            setProduct((prev) => ({ ...prev, [field]: value }))
          }
        />
        <ImageUploader
          images={product.images}
          error={errors.images}
          isLoading={isLoading || uploading}
          onImagesChange={(images: any) =>
            setProduct((prev) => ({ ...prev, images }))
          }
        />
        <VariantList
          variants={product.variants}
          error={errors.variants}
          isLoading={isLoading || uploading}
          productId={id}
          axiosInstance={axiosInstance}
          onVariantsChange={(variants: any) =>
            setProduct((prev) => ({ ...prev, variants }))
          }
          onAddVariant={handleAddVariant}
        />
        <div className="flex space-x-4">
          <Button type="submit" disabled={isLoading || uploading}>
            {isLoading || uploading ? (
              <>
                <Loader2 className="animate-spin mr-2 h-4 w-4" />
                Saving...
              </>
            ) : isEditMode ? (
              "Update Product"
            ) : (
              "Add Product"
            )}
          </Button>
          <Button
            variant="outline"
            onClick={() => navigate("/product")}
            disabled={isLoading || uploading}
          >
            Cancel
          </Button>
        </div>
      </form>
    </div>
  );
};

export default ProductForm;
