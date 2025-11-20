import { showNotification } from '../stores/notificationStore';

// Hook for testing notifications - can be used from browser console or components
export const useNotificationTest = () => {
  const testSuccess = () => {
    showNotification.success(
      'Success!', 
      'This is a success notification message.',
      5000
    );
  };

  const testError = () => {
    showNotification.error(
      'Error occurred!', 
      'This is an error notification message that is a bit longer to test text wrapping.',
      7000
    );
  };

  const testWarning = () => {
    showNotification.warning(
      'Warning!', 
      'This is a warning notification.',
      4000
    );
  };

  const testInfo = () => {
    showNotification.info(
      'Information', 
      'This is an info notification message.',
      6000
    );
  };

  const testMultiple = () => {
    showNotification.info('First notification', 'This will stack with others');
    setTimeout(() => showNotification.success('Second notification', 'Appears after delay'), 500);
    setTimeout(() => showNotification.warning('Third notification', 'Another one!'), 1000);
  };

  const testPersistent = () => {
    showNotification.error(
      'Persistent notification', 
      'This notification will not auto-dismiss.',
      0 // 0 means never auto-dismiss
    );
  };

  return {
    testSuccess,
    testError,
    testWarning,
    testInfo,
    testMultiple,
    testPersistent
  };
};

// Global functions for browser console testing
if (typeof window !== 'undefined') {
  (window as any).testNotifications = {
    success: () => showNotification.success('Console Success', 'Called from browser console!'),
    error: () => showNotification.error('Console Error', 'Called from browser console!'),
    warning: () => showNotification.warning('Console Warning', 'Called from browser console!'),
    info: () => showNotification.info('Console Info', 'Called from browser console!')
  };
}