import React from 'react';

interface ModalFooterProps {
    children: React.ReactNode;
    className?: string;
}

const ModalFooter: React.FC<ModalFooterProps> = ({ 
    children, 
    className = '' 
}) => {
    return (
        <div className={`modal-footer ${className}`}>
            {children}
        </div>
    );
};

export default ModalFooter;