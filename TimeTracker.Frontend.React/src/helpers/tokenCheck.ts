import { jwtDecode, type JwtPayload } from 'jwt-decode';

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

export default isTokenValid;