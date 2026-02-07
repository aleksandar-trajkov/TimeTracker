import React from 'react';

interface InputProps {
    id: string;
    name: string;
    type: 'text' | 'email' | 'password' | 'number' | 'tel' | 'url';
    label: string;
    value: string;
    onChange: (value: string) => void;
    placeholder?: string;
    required?: boolean;
    disabled?: boolean;
    className?: string;
    containerClassName?: string;
}

const Input: React.FC<InputProps> = ({
    id,
    name,
    type,
    label,
    value,
    onChange,
    placeholder,
    required = false,
    disabled = false,
    className = '',
    containerClassName = 'mb-3'
}) => {
    // Internal onChange handler that extracts the value
    const handleInternalChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        onChange(e.target.value);
    };

    return (
        <div className={containerClassName}>
            <label htmlFor={id} className="form-label">
                {label}
                {required && <span className="text-danger ms-1">*</span>}
            </label>
            <input
                type={type}
                id={id}
                name={name}
                className={`form-control ${className}`}
                value={value}
                onChange={handleInternalChange}
                placeholder={placeholder}
                required={required}
                disabled={disabled}
            />
        </div>
    );
};

export default Input;
