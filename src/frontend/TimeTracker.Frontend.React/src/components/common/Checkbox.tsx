import React from 'react';

interface CheckboxProps {
    id: string;
    name?: string;
    label: string;
    checked: boolean;
    onChange: (checked: boolean) => void;
    disabled?: boolean;
    className?: string;
    containerClassName?: string;
}

const Checkbox: React.FC<CheckboxProps> = ({
    id,
    name,
    label,
    checked,
    onChange,
    disabled = false,
    className = '',
    containerClassName = 'mb-3 form-check'
}) => {
    // Internal onChange handler that extracts the checked value
    const handleInternalChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        onChange(e.target.checked);
    };

    return (
        <div className={containerClassName}>
            <input
                type="checkbox"
                id={id}
                name={name}
                className={`form-check-input ${className}`}
                checked={checked}
                onChange={handleInternalChange}
                disabled={disabled}
            />
            <label className="form-check-label" htmlFor={id}>
                {label}
            </label>
        </div>
    );
};

export default Checkbox;
