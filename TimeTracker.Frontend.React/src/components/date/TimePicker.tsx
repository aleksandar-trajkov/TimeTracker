import React from 'react';

interface TimePickerProps {
    id: string;
    name: string;
    label: string;
    value: Date | null;
    onChange: (date: Date) => void;
    required?: boolean;
    disabled?: boolean;
    className?: string;
    containerClassName?: string;
}

interface TimeOption {
    value: number;
    label: string;
}

const TimePicker: React.FC<TimePickerProps> = ({ 
    id, 
    name, 
    label, 
    value, 
    onChange, 
    required = false, 
    disabled = false, 
    className = '', 
    containerClassName = 'mb-3' 
}) => {
    // Generate hour options (0-23)
    const hourOptions: TimeOption[] = Array.from({ length: 24 }, (_, i) => ({
        value: i,
        label: i.toString().padStart(2, '0')
    }));

    // Generate minute options (0, 5, 10, ..., 55)
    const minuteOptions: TimeOption[] = Array.from({ length: 12 }, (_, i) => {
        const minute = i * 5;
        return {
            value: minute,
            label: minute.toString().padStart(2, '0')
        };
    });

    // Get current hour and minute from the date
    const currentHour = value ? value.getHours() : 0;
    const currentMinute = value ? value.getMinutes() : 0;
    
    // Find closest 5-minute interval for display
    const roundedMinute = Math.round(currentMinute / 5) * 5;

    const handleHourChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newHour = parseInt(e.target.value, 10);
        if (value) {
            const newDate = new Date(value);
            newDate.setHours(newHour);
            onChange(newDate);
        } else {
            // Create new date with current date but specified hour
            const newDate = new Date();
            newDate.setHours(newHour, roundedMinute, 0, 0);
            onChange(newDate);
        }
    };

    const handleMinuteChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newMinute = parseInt(e.target.value, 10);
        if (value) {
            const newDate = new Date(value);
            newDate.setMinutes(newMinute);
            onChange(newDate);
        } else {
            // Create new date with current date but specified minute
            const newDate = new Date();
            newDate.setHours(currentHour, newMinute, 0, 0);
            onChange(newDate);
        }
    };

    return (
        <div className={containerClassName}>
            <label htmlFor={id} className="form-label">
                {label}
                {required && <span className="text-danger ms-1">*</span>}
            </label>
            <div className="row">
                <div className="col-6">
                    <select
                        id={`${id}-hour`}
                        name={`${name}-hour`}
                        className={`form-select ${className}`}
                        value={currentHour}
                        onChange={handleHourChange}
                        required={required}
                        disabled={disabled}
                    >
                        {hourOptions.map((option) => (
                            <option key={option.value} value={option.value}>
                                {option.label}
                            </option>
                        ))}
                    </select>
                    <small className="text-muted">Hours</small>
                </div>
                <div className="col-6">
                    <select
                        id={`${id}-minute`}
                        name={`${name}-minute`}
                        className={`form-select ${className}`}
                        value={roundedMinute}
                        onChange={handleMinuteChange}
                        required={required}
                        disabled={disabled}
                    >
                        {minuteOptions.map((option) => (
                            <option key={option.value} value={option.value}>
                                {option.label}
                            </option>
                        ))}
                    </select>
                    <small className="text-muted">Minutes</small>
                </div>
            </div>
        </div>
    );
};

export default TimePicker;