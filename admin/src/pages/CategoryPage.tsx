import React, { useState, useEffect } from "react";
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
  Input,
  Label,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui";
import { Pencil, Trash } from "lucide-react";
import useAxios from "@/hooks/useAxios";
import { uploadCategoryImage } from "@/lib/supabaseClient"; // Adjust the import path as necessary

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
  const [categoryName, setCategoryName] = useState("");
  const [categoryDescription, setCategoryDescription] = useState("");
  const [categoryImage, setCategoryImage] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [apiError, setApiError] = useState<string | null>(null);

  // Fetch categories on mount
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await axiosInstance.get<ApiResponse<Category[]>>(
          "/api/admin/category"
        );
        if (response.data.succeeded) {
          setCategories(response.data.data);
        } else {
          setApiError(
            response.data.errors?.join(", ") || "Failed to fetch categories"
          );
        }
      } catch (err: any) {
        setApiError(err.message || "Error fetching categories");
      }
    };

    fetchCategories();
  }, [axiosInstance]);

  // Handle image selection and preview
  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setCategoryImage(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    } else {
      setCategoryImage(null);
      setImagePreview(null);
    }
  };

  const handleOpenAdd = () => {
    setEditingCategory(null);
    setCategoryName("");
    setCategoryDescription("");
    setCategoryImage(null);
    setImagePreview(null);
    setApiError(null);
    setOpenDialog(true);
  };

  const handleEdit = (category: Category) => {
    setEditingCategory(category);
    setCategoryName(category.name);
    setCategoryDescription(category.description);
    setCategoryImage(null);
    setImagePreview(category.imageUrl || null);
    setApiError(null);
    setOpenDialog(true);
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm("Are you sure you want to delete this category?"))
      return;
    try {
      const response = await axiosInstance.delete<ApiResponse<boolean>>(
        `/api/admin/category/${id}`
      );
      if (response.data.succeeded) {
        setCategories(categories.filter((category) => category.id !== id));
      } else {
        setApiError(
          response.data.errors?.join(", ") || "Failed to delete category"
        );
      }
    } catch (err: any) {
      setApiError(err.message || "Error deleting category");
    }
  };

  const handleSave = async () => {
    if (!categoryName.trim()) {
      setApiError("Category name is required");
      return;
    }

    try {
      let imageUrl = editingCategory?.imageUrl || "";
      if (categoryImage) {
        imageUrl = await uploadCategoryImage(categoryImage);
      }

      const request: CategoryRequest = {
        name: categoryName,
        description: categoryDescription,
        imageUrl,
      };

      if (editingCategory) {
        // Update category
        const response = await axiosInstance.patch<ApiResponse<Category>>(
          `/api/admin/category/${editingCategory.id}`,
          request
        );
        if (response.data.succeeded) {
          setCategories(
            categories.map((category) =>
              category.id === editingCategory.id ? response.data.data : category
            )
          );
        } else {
          setApiError(
            response.data.errors?.join(", ") || "Failed to update category"
          );
          return;
        }
      } else {
        // Create category
        const response = await axiosInstance.post<ApiResponse<Category>>(
          "/api/admin/category",
          request
        );
        if (response.data.succeeded) {
          setCategories([...categories, response.data.data]);
        } else {
          setApiError(
            response.data.errors?.join(", ") || "Failed to create category"
          );
          return;
        }
      }
      setOpenDialog(false);
      setCategoryImage(null);
      setImagePreview(null);
    } catch (err: any) {
      setApiError(err.message || "Error saving category");
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

      {isLoading && !categories.length ? (
        <div className="text-center">Loading...</div>
      ) : (
        <Card>
          <CardHeader>
            <CardTitle>Category List</CardTitle>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Image</TableHead>
                  <TableHead>Name</TableHead>
                  <TableHead>Description</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {categories.map((category) => (
                  <TableRow key={category.id}>
                    <TableCell>
                      {category.imageUrl ? (
                        <img
                          src={category.imageUrl}
                          alt={category.name}
                          className="w-16 h-16 object-cover rounded"
                        />
                      ) : (
                        "No image"
                      )}
                    </TableCell>
                    <TableCell>{category.name}</TableCell>
                    <TableCell>{category.description}</TableCell>
                    <TableCell className="text-right space-x-2">
                      <Button
                        size="icon"
                        variant="ghost"
                        onClick={() => handleEdit(category)}
                        disabled={isLoading}
                      >
                        <Pencil className="w-4 h-4" />
                      </Button>
                      <Button
                        size="icon"
                        variant="ghost"
                        onClick={() => handleDelete(category.id)}
                        disabled={isLoading}
                      >
                        <Trash className="w-4 h-4" />
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
      )}

      <Dialog open={openDialog} onOpenChange={setOpenDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              {editingCategory ? "Edit Category" : "Add Category"}
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label>Name</Label>
              <Input
                value={categoryName}
                onChange={(e) => setCategoryName(e.target.value)}
                placeholder="Category Name"
                disabled={isLoading}
              />
            </div>
            <div>
              <Label>Description</Label>
              <Input
                value={categoryDescription}
                onChange={(e) => setCategoryDescription(e.target.value)}
                placeholder="Category Description"
                disabled={isLoading}
              />
            </div>
            <div>
              <Label>Image</Label>
              <Input
                type="file"
                accept="image/*"
                onChange={handleImageChange}
                disabled={isLoading}
              />
              {imagePreview && (
                <img
                  src={imagePreview}
                  alt="Preview"
                  className="mt-2 w-32 h-32 object-cover rounded"
                />
              )}
            </div>
            <Button
              className="w-full"
              onClick={handleSave}
              disabled={isLoading}
            >
              {isLoading
                ? "Saving..."
                : editingCategory
                ? "Save Changes"
                : "Add Category"}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}

export default CategoryPage;
