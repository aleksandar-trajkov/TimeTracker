import React from 'react';
import DatePicker from './DatePicker';
import TimePicker from './TimePicker';

interface DateTimePickerProps {
    id: string;
    name: string;
    label: string;
    value: Date | null;
    onChange: (date: Date | null) => void;
    min?: Date;
    max?: Date;
    required?: boolean;
    disabled?: boolean;
    className?: string;
    containerClassName?: string;
    placeholder?: string;
    dateLabel?: string;
    timeLabel?: string;
    showTime?: boolean;
}

const DateTimePicker: React.FC<DateTimePickerProps> = ({
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
    placeholder,
    dateLabel = 'Date',
    timeLabel = 'Time',
    showTime = true
}) => {
    const handleDateChange = (newDate: Date | null) => {
        if (newDate) {
            if (value) {
                // Preserve time from existing value
                const updatedDate = new Date(newDate);
                updatedDate.setHours(value.getHours(), value.getMinutes(), value.getSeconds(), value.getMilliseconds());
                onChange(updatedDate);
            } else {
                // Set default time (current time or midnight)
                const updatedDate = new Date(newDate);
                const now = new Date();
                updatedDate.setHours(now.getHours(), now.getMinutes(), 0, 0);
                onChange(updatedDate);
            }
        } else {
            onChange(null);
        }
    };

    const handleTimeChange = (newDate: Date) => {
        if (value) {
            // Preserve date from existing value, update time
            const updatedDate = new Date(value);
            updatedDate.setHours(newDate.getHours(), newDate.getMinutes(), 0, 0);
            onChange(updatedDate);
        } else {
            // Set date to today if no existing value
            const today = new Date();
            const updatedDate = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 
                                       newDate.getHours(), newDate.getMinutes(), 0, 0);
            onChange(updatedDate);
        }
    };

    return (
        <div className={containerClassName}>
            <label className="form-label">
                {label}
                {required && <span className="text-danger ms-1">*</span>}
            </label>
            <div className="row">
                <div className={showTime ? "col-md-7" : "col-12"}>
                    <DatePicker
                        id={`${id}-date`}
                        name={`${name}-date`}
                        label={dateLabel}
                        value={value || new Date()}
                        onChange={handleDateChange}
                        min={min}
                        max={max}
                        required={required}
                        disabled={disabled}
                        className={className}
                        containerClassName="mb-0"
                        placeholder={placeholder}
                    />
                </div>
                {showTime && (
                    <div className="col-md-5">
                        <TimePicker
                            id={`${id}-time`}
                            name={`${name}-time`}
                            label={timeLabel}
                            value={value}
                            onChange={handleTimeChange}
                            required={required}
                            disabled={disabled}
                            className={className}
                            containerClassName="mb-0"
                        />
                    </div>
                )}
            </div>
        </div>
    );
};

export default DateTimePicker;