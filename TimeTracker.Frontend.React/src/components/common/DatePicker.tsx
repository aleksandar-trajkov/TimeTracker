import React from 'react';
import { formatDate } from '../../helpers/dateTimeHelper';

interface DatePickerProps {
    id: string;
    name: string;
    label: string;
    value: Date; // ISO date string (YYYY-MM-DD)
    onChange: (value: Date) => void;
    min?: Date; // ISO date string for minimum date
    max?: Date; // ISO date string for maximum date
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
    max,
    required = false,
    disabled = false,
    className = '',
    containerClassName = 'mb-3',
    placeholder
}) => {
    const handleInternalChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        onChange(new Date(e.target.value));
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
                value={formatDate(value)}
                onChange={handleInternalChange}
                min={min ? formatDate(min) : undefined}
                max={max ? formatDate(max) : undefined}
                required={required}
                disabled={disabled}
                placeholder={placeholder}
            />
        </div>
    );
};

export default DatePicker;