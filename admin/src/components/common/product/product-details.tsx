import React from "react";
import {
  Input,
  Textarea,
  Label,
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui";

interface Category {
  id: string;
  name: string;
}

interface ProductDetailsProps {
  product: {
    name: string;
    description: string;
    categoryId: string;
    isParent: boolean;
  };
  categories: Category[];
  errors: {
    name?: string;
    categoryId?: string;
  };
  onChange: (field: keyof ProductDetailsProps["product"], value: any) => void;
}

const ProductDetails: React.FC<ProductDetailsProps> = ({
  product,
  categories,
  errors,
  onChange,
}) => {
  return (
    <div className="space-y-6">
      {/* Name */}
      <div className="flex w-full gap-4 items-center">
        <div className="flex-2/3">
          <Label className="mb-2" htmlFor="name">
            Product Name
          </Label>

          <Input
            id="name"
            value={product.name}
            onChange={(e) => onChange("name", e.target.value)}
            placeholder="Enter product name"
          />
          {errors.name && (
            <p className="text-red-500 text-sm mt-1">{errors.name}</p>
          )}
        </div>
        <div className="flex-1/3">
          <Label className="mb-2" htmlFor="category">
            Category
          </Label>
          <Select
            value={product.categoryId}
            onValueChange={(value) => onChange("categoryId", value)}
          >
            <SelectTrigger id="category">
              <SelectValue placeholder="Select a category" />
            </SelectTrigger>
            <SelectContent>
              {categories.map((category) => (
                <SelectItem key={category.id} value={category.id}>
                  {category.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          {errors.categoryId && (
            <p className="text-red-500 text-sm mt-1">{errors.categoryId}</p>
          )}
        </div>
      </div>

      {/* Description */}
      <div>
        <Label className="mb-2" htmlFor="description">
          Description
        </Label>
        <Textarea
          id="description"
          value={product.description}
          onChange={(e) => onChange("description", e.target.value)}
          placeholder="Enter product description"
        />
      </div>

      {/* Category */}

      {/* Is Parent */}
      {/* <div className="flex items-center space-x-2">
        <Checkbox
          id="isParent"
          checked={product.isParent}
          onCheckedChange={(checked) => onChange("isParent", !!checked)}
        />
        <Label className="mb-2" htmlFor="isParent">
          Is Parent Product
        </Label>
      </div> */}
    </div>
  );
};

export default ProductDetails;
