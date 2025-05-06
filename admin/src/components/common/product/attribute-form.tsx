import React from "react";
import { Input, Button, Label } from "@/components/ui";

interface VariantAttribute {
  name: string;
  value: string;
}

interface AttributeFormProps {
  attributes: VariantAttribute[];
  isLoading: boolean;
  onAttributesChange: (attributes: VariantAttribute[]) => void;
}

const AttributeForm: React.FC<AttributeFormProps> = ({
  attributes,
  isLoading,
  onAttributesChange,
}) => {
  const handleAddAttribute = () => {
    onAttributesChange([...attributes, { name: "", value: "" }]);
  };

  const handleRemoveAttribute = (index: number) => {
    onAttributesChange(attributes.filter((_, i) => i !== index));
  };

  const handleAttributeChange = (
    index: number,
    field: keyof VariantAttribute,
    value: string
  ) => {
    onAttributesChange(
      attributes.map((attr, i) =>
        i === index ? { ...attr, [field]: value } : attr
      )
    );
  };

  return (
    <div>
      <Label className="mb-2">Attributes</Label>
      {attributes.map((attr, index) => (
        <div key={index} className="flex space-x-2 mt-2">
          <Input
            placeholder="Attribute name"
            value={attr.name}
            onChange={(e) =>
              handleAttributeChange(index, "name", e.target.value)
            }
            disabled={isLoading}
          />
          <Input
            placeholder="Attribute value"
            value={attr.value}
            onChange={(e) =>
              handleAttributeChange(index, "value", e.target.value)
            }
            disabled={isLoading}
          />
          <Button
            variant="destructive"
            size="sm"
            onClick={() => handleRemoveAttribute(index)}
            disabled={isLoading}
          >
            Remove
          </Button>
        </div>
      ))}
      <Button
        type="button"
        variant="outline"
        size="sm"
        onClick={handleAddAttribute}
        className="mt-2"
        disabled={isLoading}
      >
        Add Attribute
      </Button>
    </div>
  );
};

export default AttributeForm;
