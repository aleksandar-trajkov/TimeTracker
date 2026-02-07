import React from 'react';

interface ModalHeaderProps {
    title: string;
    onClose: () => void;
    className?: string;
}

const ModalHeader: React.FC<ModalHeaderProps> = ({ 
    title, 
    onClose, 
    className = '' 
}) => {
    return (
        <div className={`modal-header ${className}`}>
            <h5 className="modal-title">{title}</h5>
            <button 
                type="button" 
                className="btn-close" 
                aria-label="Close"
                onClick={onClose}
            />
        </div>
    );
};

export default ModalHeader;