import React from 'react';

export interface DropdownOption {
    value: string | number;
    label: string;
    disabled?: boolean;
}

interface DropdownProps {
    id: string;
    name: string;
    label: string;
    value: string | number;
    options: DropdownOption[];
    onChange: (value: string | number) => void;
    placeholder?: string;
    required?: boolean;
    disabled?: boolean;
    className?: string;
    containerClassName?: string;
    allowEmpty?: boolean;
    emptyOptionLabel?: string;
}

const Dropdown: React.FC<DropdownProps> = ({
    id,
    name,
    label,
    value,
    options,
    onChange,
    placeholder,
    required = false,
    disabled = false,
    className = '',
    containerClassName = 'mb-3',
    allowEmpty = false,
    emptyOptionLabel = 'Select an option...'
}) => {
    // Internal onChange handler that extracts the value
    const handleInternalChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedValue = e.target.value;
        
        // Find the option to determine the original type
        const selectedOption = options.find(option => String(option.value) === selectedValue);
        if (selectedOption) {
            onChange(selectedOption.value);
        } else {
            // Fallback for empty values or edge cases
            onChange(selectedValue);
        }
    };

    return (
        <div className={containerClassName}>
            <label htmlFor={id} className="form-label">
                {label}
                {required && <span className="text-danger ms-1">*</span>}
            </label>
            <select
                id={id}
                name={name}
                className={`form-select ${className}`}
                value={value}
                onChange={handleInternalChange}
                required={required}
                disabled={disabled}
            >
                {(allowEmpty || (!required && value === '')) && (
                    <option value="" disabled={required}>
                        {placeholder || emptyOptionLabel}
                    </option>
                )}
                {options.map((option) => (
                    <option 
                        key={option.value} 
                        value={option.value}
                        disabled={option.disabled}
                    >
                        {option.label}
                    </option>
                ))}
            </select>
        </div>
    );
};

export default Dropdown;