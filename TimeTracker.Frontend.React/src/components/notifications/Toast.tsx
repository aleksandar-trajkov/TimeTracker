import React, { useEffect, useState } from 'react';
import type { Notification } from '../../stores/notificationStore';

interface ToastProps {
  notification: Notification;
  onClose: (id: string) => void;
}

const Toast: React.FC<ToastProps> = ({ notification, onClose }) => {
  const [isVisible, setIsVisible] = useState(false);
  const [isLeaving, setIsLeaving] = useState(false);

  useEffect(() => {
    // Trigger the entrance animation
    const timer = setTimeout(() => setIsVisible(true), 10);
    return () => clearTimeout(timer);
  }, []);

  const handleClose = () => {
    setIsLeaving(true);
    setTimeout(() => {
      onClose(notification.id);
    }, 300); // Match animation duration
  };

  const getToastClasses = () => {
    const baseClasses = 'toast-notification';
    const typeClasses = {
      success: 'toast-success',
      error: 'toast-error',
      warning: 'toast-warning',
      info: 'toast-info'
    };
    
    let classes = `${baseClasses} ${typeClasses[notification.type]}`;
    
    if (isVisible && !isLeaving) {
      classes += ' toast-enter';
    } else if (isLeaving) {
      classes += ' toast-exit';
    }
    
    return classes;
  };

  const getIcon = () => {
    switch (notification.type) {
      case 'success':
        return '✓';
      case 'error':
        return '✕';
      case 'warning':
        return '⚠';
      case 'info':
        return 'ℹ';
      default:
        return 'ℹ';
    }
  };

  return (
    <div className={getToastClasses()}>
      <div className="toast-content">
        <div className="toast-icon">
          {getIcon()}
        </div>
        <div className="toast-text">
          <div className="toast-title">{notification.title}</div>
          {notification.message && (
            <div className="toast-message">{notification.message}</div>
          )}
        </div>
        <button
          className="toast-close"
          onClick={handleClose}
          type="button"
          aria-label="Close notification"
        >
          ×
        </button>
      </div>
    </div>
  );
};

export default Toast;