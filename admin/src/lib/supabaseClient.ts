import { createClient } from "@supabase/supabase-js";

const supabaseUrl = import.meta.env.VITE_SUPABASE_URL;
const supabaseAnonKey = import.meta.env.VITE_SUPABASE_ANON_KEY;

if (!supabaseUrl || !supabaseAnonKey) {
  throw new Error("Supabase URL and Anon Key must be defined in .env");
}

export const supabase = createClient(supabaseUrl, supabaseAnonKey);

// Upload image to Supabase Storage and return public URL
export const uploadCategoryImage = async (file: File): Promise<string> => {
  const fileExt = file.name.split(".").pop();
  const fileName = `${Date.now()}.${fileExt}`; // Unique filename
  const filePath = `category-images/${fileName}`; // Store in a folder

  console.log("Uploading image to nt-bucket:", {
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
      throw new Error(`Failed to upload image: ${error.message}`);
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
    throw new Error(`Image upload failed: ${err.message}`);
  }
};

// Update (replace) an existing image in Supabase Storage
export const updateCategoryImage = async (
  file: File,
  existingPath: string
): Promise<string> => {
  console.log("Updating image in nt-bucket:", {
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
      throw new Error(`Failed to update image: ${error.message}`);
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
    throw new Error(`Image update failed: ${err.message}`);
  }
};

// Delete an image from Supabase Storage
export const deleteCategoryImage = async (filePath: string): Promise<void> => {
  console.log("Deleting image from nt-bucket:", filePath);

  try {
    const { error } = await supabase.storage
      .from("nt-bucket")
      .remove([filePath]);

    if (error) {
      console.error("Supabase delete error:", error);
      throw new Error(`Failed to delete image: ${error.message}`);
    }

    console.log("Image deleted successfully:", filePath);
  } catch (err: any) {
    console.error("DeleteCategoryImage error:", err);
    throw new Error(`Image deletion failed: ${err.message}`);
  }
};
