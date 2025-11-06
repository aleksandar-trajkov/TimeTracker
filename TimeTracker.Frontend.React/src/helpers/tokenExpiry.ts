/**
 * Calculate token expiry in days based on environment variable
 * @param defaultHours - Default expiry hours if environment variable is not set
 * @returns Token expiry in days for js-cookie
 */
export const calculateTokenExpiry = (defaultHours: number = 2): number => {
    var expiryHours = import.meta.env.VITE_TOKEN_EXPIRY_HOURS;
    const tokenExpiryHours = expiryHours === '' || isNaN(expiryHours) ? defaultHours : Number(expiryHours);
    return tokenExpiryHours / 24; // Convert hours to days for js-cookie
};

/**
 * Get token expiry for remember me tokens (14 days)
 * @returns Remember me token expiry in days
 */
export const getRememberMeExpiry = (): number => {
    return 14; // 14 days for remember me tokens
};
