export interface SignInRequest {
    email: string;
    password: string;
    rememberMe: boolean;
}

export interface TokenResponse {
    token: string;
    rememberMeToken?: string;
}

export interface UseSignInMutationProps {
    setIsSignedIn: React.Dispatch<React.SetStateAction<boolean>>;
}

export interface UseRememberMeSignInMutationProps {
    setIsSignedIn: React.Dispatch<React.SetStateAction<boolean>>;
}