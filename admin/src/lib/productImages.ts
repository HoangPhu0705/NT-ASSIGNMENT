/* eslint-disable @typescript-eslint/no-explicit-any */
import { supabase } from "./supaBaseClient";

export const uploadProductImage = async (file: File): Promise<string> => {
  const fileExt = file.name.split(".").pop();
  const fileName = `${Date.now()}.${fileExt}`; // Unique filename
  const filePath = `product-images/${fileName}`; // Store in a folder

  console.log("Uploading product image to nt-bucket:", {
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
      throw new Error(`Failed to upload product image: ${error.message}`);
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
    console.error("UploadProductImage error:", err);
    throw new Error(`Product image upload failed: ${err.message}`);
  }
};

export const updateProductImage = async (
  file: File,
  existingPath: string
): Promise<string> => {
  console.log("Updating product image in nt-bucket:", {
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
      throw new Error(`Failed to update product image: ${error.message}`);
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
    console.error("UpdateProductImage error:", err);
    throw new Error(`Product image update failed: ${err.message}`);
  }
};

export const deleteProductImage = async (filePath: string): Promise<void> => {
  console.log("Deleting product image from nt-bucket:", filePath);

  try {
    const { error } = await supabase.storage
      .from("nt-bucket")
      .remove([filePath]);

    if (error) {
      console.error("Supabase delete error:", error);
      throw new Error(`Failed to delete product image: ${error.message}`);
    }

    console.log("Product image deleted successfully:", filePath);
  } catch (err: any) {
    console.error("DeleteProductImage error:", err);
    throw new Error(`Product image deletion failed: ${err.message}`);
  }
};
