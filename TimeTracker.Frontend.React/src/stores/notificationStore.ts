import { create } from "zustand";

// Simple ID generator
const generateId = () => `notification_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;

export interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message?: string;
  duration?: number; // in milliseconds, 0 means never auto-dismiss
  timestamp: Date;
}

// Type for API errors that may contain multiple error messages
interface ApiError {
  message?: string;
  errors?: string[];
  [key: string]: unknown;
}

interface NotificationState {
  notifications: Notification[];
  addNotification: (notification: Omit<Notification, 'id' | 'timestamp'>) => string;
  removeNotification: (id: string) => void;
  clearAllNotifications: () => void;
}

const useNotificationStore = create<NotificationState>((set, get) => ({
  notifications: [],
  
  addNotification: (notification) => {
    const id = generateId();
    const newNotification: Notification = {
      ...notification,
      id,
      timestamp: new Date(),
      duration: notification.duration ?? 5000, // Default 5 seconds
    };

    set((state) => ({
      notifications: [...state.notifications, newNotification]
    }));

    // Auto-dismiss if duration is set
    if (newNotification.duration && newNotification.duration > 0) {
      setTimeout(() => {
        get().removeNotification(id);
      }, newNotification.duration);
    }

    return id;
  },
  
  removeNotification: (id) => {
    set((state) => ({
      notifications: state.notifications.filter((n) => n.id !== id)
    }));
  },
  
  clearAllNotifications: () => {
    set({ notifications: [] });
  }
}));

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
  errorList: (title: string, errors: string[], duration?: number) => {
    const store = useNotificationStore.getState();
    
    if (!errors || errors.length === 0) {
      // Fallback to single error if no errors provided
      return store.addNotification({ type: 'error', title, message: 'An error occurred', duration });
    }
    
    if (errors.length === 1) {
      // Single error, show as regular error notification
      return store.addNotification({ type: 'error', title, message: errors[0], duration });
    }
    
    // Multiple errors, show as a single notification with formatted message
    const formattedMessage = errors.map((error) => `${error}`).join('\n');
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

export default useNotificationStore;