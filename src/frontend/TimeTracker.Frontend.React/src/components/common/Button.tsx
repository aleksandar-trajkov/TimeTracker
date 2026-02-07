import React from 'react';

interface ButtonProps {
    type?: 'button' | 'submit' | 'reset';
    variant?: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'light' | 'dark' | 'outline-primary' | 'outline-secondary';
    size?: 'sm' | 'lg';
    disabled?: boolean;
    loading?: boolean;
    loadingText?: string;
    className?: string;
    children: React.ReactNode;
    onClick?: () => void;
    fullWidth?: boolean;
}

const Button: React.FC<ButtonProps> = ({
    type = 'button',
    variant = 'primary',
    size,
    disabled = false,
    loading = false,
    loadingText = 'Loading...',
    className = '',
    children,
    onClick,
    fullWidth = false
}) => {
    const sizeClass = size ? `btn-${size}` : '';
    const widthClass = fullWidth ? 'w-100' : '';
    const buttonClasses = `btn btn-${variant} ${sizeClass} ${widthClass} ${className}`.trim();

    return (
        <button
            type={type}
            className={buttonClasses}
            disabled={disabled || loading}
            onClick={onClick}
        >
            {loading ? (
                <>
                    <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    {loadingText}
                </>
            ) : (
                children
            )}
        </button>
    );
};

export default Button;
