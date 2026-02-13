import useNotificationStore from '../stores/notificationStore';

// Type for API errors that may contain multiple error messages
interface ApiError {
  message?: string;
  errors?: ApiValidationError[];
  [key: string]: unknown;
}

interface ApiValidationError {
  property: string;
  message: string;
}

// Helper functions for common notification types
export const showNotification = {
  success: (title: string, message?: string, duration?: number) =>
    useNotificationStore.getState().addNotification({ type: 'success', title, message, duration }),
  
  error: (title: string, message?: string, duration?: number) =>
    useNotificationStore.getState().addNotification({ type: 'error', title, message, duration }),
  
  warning: (title: string, message?: string, duration?: number) =>
    useNotificationStore.getState().addNotification({ type: 'warning', title, message, duration }),
  
  info: (title: string, message?: string, duration?: number) =>
    useNotificationStore.getState().addNotification({ type: 'info', title, message, duration }),

  // Show multiple errors with the same title
  errorList: (title: string, errors: ApiValidationError[], duration?: number) => {
    const store = useNotificationStore.getState();
    
    if (!errors || errors.length === 0) {
      // Fallback to single error if no errors provided
      return store.addNotification({ type: 'error', title, message: 'An error occurred', duration });
    }
    
    if (errors.length === 1) {
      // Single error, show as regular error notification
      return store.addNotification({ type: 'error', title, message: errors[0].message, duration });
    }
    
    // Multiple errors, show as a single notification with formatted message
    const formattedMessage = errors.map((error, index) => `${index + 1}. ${error.message}`).join('\n');
    return store.addNotification({ 
      type: 'error', 
      title, 
      message: formattedMessage, 
      duration: duration ?? 7000 // Longer duration for multiple errors
    });
  },

  // Helper to handle API errors automatically
  apiError: (title: string, error: ApiError | Error | unknown, fallbackMessage: string = 'An error occurred') => {
    const apiError = error as ApiError;
    if (apiError?.errors && Array.isArray(apiError.errors) && apiError.errors.length > 0) {
      return showNotification.errorList(title, apiError.errors);
    } else {
      const message = apiError?.message || (error as Error)?.message || fallbackMessage;
      return showNotification.error(title, message);
    }
  }
};