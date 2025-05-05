/* eslint-disable @typescript-eslint/no-unused-vars */
import React from "react";
import toast from "react-hot-toast";
import { Input, Label, Card, CardContent } from "@/components/ui";
import { Trash2 } from "lucide-react";

interface ProductImage {
  id?: string;
  imageUrl: string;
  isPrimary: boolean;
  isDeleted?: boolean;
  file?: File;
}

interface ImageUploaderProps {
  images: ProductImage[];
  error?: string;
  isLoading: boolean;
  onImagesChange: (images: ProductImage[]) => void;
}

const ImageUploader: React.FC<ImageUploaderProps> = ({
  images,
  error,
  isLoading,
  onImagesChange,
}) => {
  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (!files) return;

    const newImages: ProductImage[] = [];
    const activeImages = images.filter((img) => !img.isDeleted);
    const totalImages = activeImages.length + files.length;

    if (totalImages > 4) {
      toast.error("Maximum 4 images allowed");
      return;
    }

    for (let i = 0; i < files.length; i++) {
      newImages.push({
        imageUrl: URL.createObjectURL(files[i]),
        isPrimary: activeImages.length === 0 && i === 0,
        file: files[i],
        isDeleted: false,
      });
    }

    onImagesChange([...images, ...newImages]);
  };

  const handleRemoveImage = (index: number) => {
    const newImages = images.map((img, i) =>
      i === index ? { ...img, isDeleted: true } : img
    );
    const activeImages = newImages.filter((img) => !img.isDeleted);
    if (activeImages.length > 0 && !activeImages.some((img) => img.isPrimary)) {
      newImages.forEach((img, i) => {
        if (!img.isDeleted && activeImages[0] === img) {
          newImages[i].isPrimary = true;
        }
      });
    }
    onImagesChange(newImages);
  };

  const handleSetPrimaryImage = (index: number) => {
    if (images[index].isDeleted) return;
    onImagesChange(
      images.map((img, i) => ({
        ...img,
        isPrimary: i === index,
      }))
    );
  };

  return (
    <div>
      <Label className="mb-2">Images (Max 4)</Label>
      <Input
        type="file"
        accept="image/*"
        multiple
        onChange={handleImageChange}
        disabled={isLoading}
      />
      {error && <p className="text-red-500 text-sm mt-1">{error}</p>}
      <div className="flex flex-row gap-4 mt-4 overflow-x-auto">
        {images.map((image, index) => {
          if (image.isDeleted) return null; // Skip rendering deleted images
          return (
            <div key={index} className="flex flex-col items-center">
              <Card
                className={`relative flex-shrink-0 w-32 cursor-pointer ${
                  image.isPrimary
                    ? "border-2 border-green-500"
                    : "border border-gray-300"
                }`}
                onClick={() => handleSetPrimaryImage(index)}
              >
                <CardContent className="p-2 flex flex-col h-full">
                  <img
                    src={image.imageUrl}
                    alt={`Image ${index + 1}`}
                    className="w-full object-contain rounded mb-2 flex-grow"
                  />
                  <div className="flex justify-center mt-auto">
                    <button
                      onClick={(e) => {
                        e.stopPropagation(); // Prevent card click from setting primary
                        handleRemoveImage(index);
                      }}
                      disabled={isLoading}
                      className="text-red-500 hover:text-red-700 mt-5"
                    >
                      <Trash2 className="h-5 w-5" />
                    </button>
                  </div>
                </CardContent>
              </Card>
              {image.isPrimary && (
                <p className="text-sm text-center mt-1 text-green-500">
                  Primary Image
                </p>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default ImageUploader;
