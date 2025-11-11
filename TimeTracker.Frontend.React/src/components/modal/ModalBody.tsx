import React from 'react';

interface ModalBodyProps {
    children: React.ReactNode;
    className?: string;
}

const ModalBody: React.FC<ModalBodyProps> = ({ 
    children, 
    className = '' 
}) => {
    return (
        <div className={`modal-body ${className}`}>
            {children}
        </div>
    );
};

export default ModalBody;