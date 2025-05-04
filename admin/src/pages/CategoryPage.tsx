/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState, useEffect } from "react";
import { Button } from "@/components/ui";
import useAxios from "@/hooks/useAxios";
import {
  uploadCategoryImage,
  updateCategoryImage,
  deleteCategoryImage,
} from "@/lib/categoryImages";
import CategoryDialog from "@/components/dialog/category-dialog";
import CategoryTable from "@/components/common/category/category-table";
import ConfirmDialog from "@/components/dialog/confirm-dialog";
import toast from "react-hot-toast";

interface Category {
  id: string;
  name: string;
  description: string;
  imageUrl: string;
}

interface CategoryRequest {
  name: string;
  description: string;
  imageUrl: string;
}

interface ApiResponse<T> {
  data: T;
  succeeded: boolean;
  errors: string[] | null;
}

function CategoryPage() {
  const { axiosInstance, isLoading, error } = useAxios();
  const [categories, setCategories] = useState<Category[]>([]);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);
  const [confirmDelete, setConfirmDelete] = useState<string | null>(null);
  const [apiError, setApiError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await axiosInstance.get<ApiResponse<Category[]>>(
          "/category"
        );
        if (response.data.succeeded) {
          setCategories(response.data.data);
        } else {
          setApiError(
            response.data.errors?.join(", ") || "Failed to fetch categories"
          );
          toast.error("Failed to fetch categories");
        }
      } catch (err: any) {
        setApiError(err.message || "Error fetching categories");
        toast.error("Error fetching categories");
      }
    };

    fetchCategories();
  }, [axiosInstance]);

  const handleOpenAdd = () => {
    setEditingCategory(null);
    setOpenDialog(true);
  };

  const handleEdit = (category: Category) => {
    setEditingCategory(category);
    setOpenDialog(true);
  };

  const handleDelete = async () => {
    if (!confirmDelete) return;
    try {
      const category = categories.find((c) => c.id === confirmDelete);
      if (category?.imageUrl) {
        const filePath = category.imageUrl.split("/").slice(-2).join("/");
        await deleteCategoryImage(filePath);
      }

      const response = await axiosInstance.delete<ApiResponse<boolean>>(
        `/category/${confirmDelete}`
      );
      if (response.data.succeeded) {
        setCategories(categories.filter((c) => c.id !== confirmDelete));
        toast.success("Category deleted successfully");
      } else {
        setApiError(
          response.data.errors?.join(", ") || "Failed to delete category"
        );
        toast.error("Failed to delete category");
      }
    } catch (err: any) {
      setApiError(err.message || "Error deleting category");
      toast.error("Error deleting category");
    } finally {
      setConfirmDelete(null);
    }
  };

  const handleSave = async (data: {
    name: string;
    description: string;
    image: File | null;
  }) => {
    try {
      let imageUrl = editingCategory?.imageUrl || "";
      if (data.image) {
        if (editingCategory && editingCategory.imageUrl) {
          const filePath = editingCategory.imageUrl
            .split("/")
            .slice(-2)
            .join("/");
          imageUrl = await updateCategoryImage(data.image, filePath);
        } else {
          imageUrl = await uploadCategoryImage(data.image);
        }
      }

      const request: CategoryRequest = {
        name: data.name,
        description: data.description,
        imageUrl,
      };

      if (editingCategory) {
        const response = await axiosInstance.patch<ApiResponse<Category>>(
          `/category/${editingCategory.id}`,
          request
        );
        if (response.data.succeeded) {
          setCategories(
            categories.map((c) =>
              c.id === editingCategory.id ? response.data.data : c
            )
          );
          toast.success("Category updated successfully");
        } else {
          throw new Error(
            response.data.errors?.join(", ") || "Failed to update category"
          );
        }
      } else {
        const response = await axiosInstance.post<ApiResponse<Category>>(
          "/category",
          request
        );
        if (response.data.succeeded) {
          setCategories([...categories, response.data.data]);
          toast.success("Category created successfully");
        } else {
          throw new Error(
            response.data.errors?.join(", ") || "Failed to create category"
          );
        }
      }
    } catch (err: any) {
      toast.error(err.message || "Error saving category");
      throw new Error(err.message || "Error saving category");
    }
  };

  return (
    <div className="p-6 space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Categories</h1>
        <Button onClick={handleOpenAdd} disabled={isLoading}>
          Add Category
        </Button>
      </div>

      {(error || apiError) && (
        <div className="p-4 bg-red-100 text-red-700 rounded">
          {error || apiError}
        </div>
      )}

      <CategoryTable
        categories={categories}
        isLoading={isLoading}
        onEdit={handleEdit}
        onDelete={(id) => setConfirmDelete(id)}
      />

      <CategoryDialog
        open={openDialog}
        onOpenChange={setOpenDialog}
        category={editingCategory}
        onSave={handleSave}
        isLoading={isLoading}
      />

      <ConfirmDialog
        open={!!confirmDelete}
        onOpenChange={() => setConfirmDelete(null)}
        onConfirm={handleDelete}
        title="Delete Category"
        description="Are you sure you want to delete this category? This action cannot be undone."
        isLoading={isLoading}
      />
    </div>
  );
}

export default CategoryPage;
