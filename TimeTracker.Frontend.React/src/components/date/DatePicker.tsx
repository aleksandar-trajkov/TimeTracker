import React from 'react';
import { formatDate } from '../../helpers/dateTimeHelper';

interface DatePickerProps {
    id: string;
    name: string;
    label: string;
    value: string; // ISO date string (YYYY-MM-DD)
    onChange: (value: string) => void;
    min?: string; // ISO date string for minimum date
    max?: string; // ISO date string for maximum date
    required?: boolean;
    disabled?: boolean;
    className?: string;
    containerClassName?: string;
    placeholder?: string;
}

const DatePicker: React.FC<DatePickerProps> = ({
    id,
    name,
    label,
    value,
    onChange,
    min,
    max = formatDate(new Date('2099-12-31')),
    required = false,
    disabled = false,
    className = '',
    containerClassName = 'mb-3',
    placeholder
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
                type="date"
                id={id}
                name={name}
                className={`form-control ${className}`}
                value={value}
                onChange={handleInternalChange}
                min={min}
                max={max}
                required={required}
                disabled={disabled}
                placeholder={placeholder}
            />
        </div>
    );
};

export default DatePicker;