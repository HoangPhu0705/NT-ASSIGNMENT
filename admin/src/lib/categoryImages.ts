/* eslint-disable @typescript-eslint/no-explicit-any */
import { supabase } from "./supabaseClient";

export const uploadCategoryImage = async (file: File): Promise<string> => {
  const fileExt = file.name.split(".").pop();
  const fileName = `${Date.now()}.${fileExt}`; // Unique filename
  const filePath = `category-images/${fileName}`; // Store in a folder

  console.log("Uploading category image to nt-bucket:", {
    filePath,
    fileSize: file.size,
    fileType: file.type,
  });

  try {
    const { error } = await supabase.storage
      .from("nt-bucket")
      .upload(filePath, file, {
        cacheControl: "3600",
        upsert: false,
      });

    if (error) {
      console.error("Supabase upload error:", error);
      throw new Error(`Failed to upload category image: ${error.message}`);
    }

    const { data } = supabase.storage.from("nt-bucket").getPublicUrl(filePath);

    if (!data.publicUrl) {
      console.error("Failed to retrieve public URL for:", filePath);
      throw new Error("Failed to retrieve public URL");
    }

    const cacheBustedUrl = `${data.publicUrl}?t=${Date.now()}`;
    console.log("Public URL generated:", cacheBustedUrl);
    return cacheBustedUrl;
  } catch (err: any) {
    console.error("UploadCategoryImage error:", err);
    throw new Error(`Category image upload failed: ${err.message}`);
  }
};

export const updateCategoryImage = async (
  file: File,
  existingPath: string
): Promise<string> => {
  console.log("Updating category image in nt-bucket:", {
    existingPath,
    fileSize: file.size,
    fileType: file.type,
  });

  try {
    const { error } = await supabase.storage
      .from("nt-bucket")
      .upload(existingPath, file, {
        cacheControl: "3600",
        upsert: true, // Overwrite existing file
      });

    if (error) {
      console.error("Supabase update error:", error);
      throw new Error(`Failed to update category image: ${error.message}`);
    }

    const { data } = supabase.storage
      .from("nt-bucket")
      .getPublicUrl(existingPath);

    if (!data.publicUrl) {
      console.error("Failed to retrieve public URL for:", existingPath);
      throw new Error("Failed to retrieve public URL");
    }

    const cacheBustedUrl = `${data.publicUrl}?t=${Date.now()}`;
    console.log("Updated public URL:", cacheBustedUrl);
    return cacheBustedUrl;
  } catch (err: any) {
    console.error("UpdateCategoryImage error:", err);
    throw new Error(`Category image update failed: ${err.message}`);
  }
};

export const deleteCategoryImage = async (filePath: string): Promise<void> => {
  console.log("Deleting category image from nt-bucket:", filePath);

  try {
    const { error } = await supabase.storage
      .from("nt-bucket")
      .remove([filePath]);

    if (error) {
      console.error("Supabase delete error:", error);
      throw new Error(`Failed to delete category image: ${error.message}`);
    }

    console.log("Category image deleted successfully:", filePath);
  } catch (err: any) {
    console.error("DeleteCategoryImage error:", err);
    throw new Error(`Category image deletion failed: ${err.message}`);
  }
};
