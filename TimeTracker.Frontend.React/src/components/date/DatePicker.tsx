import React from 'react';
import { formatDate, isIsoDateString } from '../../helpers/dateTime';

interface DatePickerProps {
    id: string;
    name: string;
    label: string;
    value: Date; // Date object (YYYY-MM-DD)
    onChange: (value: Date | null) => void;
    min?: Date; // Date object for minimum date
    max?: Date; // Date object for maximum date
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
    max = new Date('2099-12-31'),
    required = false,
    disabled = false,
    className = '',
    containerClassName = 'mb-3',
    placeholder
}) => {
    // Internal onChange handler that extracts the value
    const handleInternalChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        onChange(isIsoDateString(e.target.value) ? new Date(e.target.value) : null);
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