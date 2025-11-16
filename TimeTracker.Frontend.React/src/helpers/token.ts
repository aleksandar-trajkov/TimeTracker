import { jwtDecode, type JwtPayload } from 'jwt-decode';

/**
 * Check if a JWT token is expired
 * @param token - JWT token string to validate
 * @returns true if token is expired, false if valid
 * @throws Error if invalid token provided
 */
const isTokenValid = (token : string) => {
    if (typeof(token) !== 'string' || !token || !token.trim()) throw new Error('Invalid token provided');

  let isJwtExpired = false;
  const { exp } = jwtDecode<JwtPayload>(token);
  const currentTime = new Date().getTime() / 1000;

  if (typeof exp === 'number' && currentTime >= exp) {
    isJwtExpired = true;
  }

  return isJwtExpired;
};

/**
 * Calculate token expiry in days based on environment variable
 * @param defaultHours - Default expiry hours if environment variable is not set
 * @returns Token expiry in days for js-cookie
 */
export const calculateTokenExpiry = (defaultHours: number = 2): number => {
    const expiryHours = import.meta.env.VITE_TOKEN_EXPIRY_HOURS;
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

export default isTokenValid;