import React from "react";
import {
  Button,
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui";
import { Pencil, Trash } from "lucide-react";

interface Category {
  id: string;
  name: string;
  description: string;
  imageUrl: string;
}

interface CategoryTableProps {
  categories: Category[];
  isLoading: boolean;
  onEdit: (category: Category) => void;
  onDelete: (id: string) => void;
}

const CategoryTable: React.FC<CategoryTableProps> = ({
  categories,
  isLoading,
  onEdit,
  onDelete,
}) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Category List</CardTitle>
      </CardHeader>
      <CardContent>
        {isLoading && !categories.length ? (
          <div className="text-center">Loading...</div>
        ) : (
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
                      onClick={() => onEdit(category)}
                      disabled={isLoading}
                    >
                      <Pencil className="w-4 h-4" />
                    </Button>
                    <Button
                      size="icon"
                      variant="ghost"
                      onClick={() => onDelete(category.id)}
                      disabled={isLoading}
                    >
                      <Trash className="w-4 h-4" />
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </CardContent>
    </Card>
  );
};

export default CategoryTable;
