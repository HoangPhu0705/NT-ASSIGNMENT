import React, { useState, useEffect } from "react";
import {
  Button,
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  Input,
  Label,
} from "@/components/ui";

interface Category {
  id: string;
  name: string;
  description: string;
  imageUrl: string;
}

interface CategoryDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  category: Category | null;
  onSave: (data: {
    name: string;
    description: string;
    image: File | null;
  }) => Promise<void>;
  isLoading: boolean;
}

const CategoryDialog: React.FC<CategoryDialogProps> = ({
  open,
  onOpenChange,
  category,
  onSave,
  isLoading,
}) => {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [image, setImage] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (category) {
      setName(category.name);
      setDescription(category.description);
      setImagePreview(category.imageUrl || null);
    } else {
      setName("");
      setDescription("");
      setImage(null);
      setImagePreview(null);
    }
    setError(null);
  }, [category, open]);

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      const validTypes = ["image/jpeg", "image/png", "image/gif"];
      if (!validTypes.includes(file.type)) {
        setError("Only JPEG, PNG, or GIF images are allowed");
        return;
      }
      if (file.size > 5 * 1024 * 1024) {
        setError("Image size must be less than 5MB");
        return;
      }
      setImage(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
      setError(null);
    } else {
      setImage(null);
      setImagePreview(null);
    }
  };

  const handleSubmit = async () => {
    if (!name.trim()) {
      setError("Category name is required");
      return;
    }
    try {
      await onSave({ name, description, image });
      onOpenChange(false);
    } catch (err: any) {
      setError(err.message || "Error saving category");
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {category ? "Edit Category" : "Add Category"}
          </DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          {error && (
            <div className="p-2 bg-red-100 text-red-700 rounded">{error}</div>
          )}
          <div>
            <Label className="mb-2">Name</Label>
            <Input
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Category Name"
              disabled={isLoading}
            />
          </div>
          <div>
            <Label className="mb-2">Description</Label>
            <Input
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Category Description"
              disabled={isLoading}
            />
          </div>
          <div>
            <Label className="mb-2">Image</Label>
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
            onClick={handleSubmit}
            disabled={isLoading}
          >
            {isLoading
              ? "Saving..."
              : category
              ? "Save Changes"
              : "Add Category"}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default CategoryDialog;
