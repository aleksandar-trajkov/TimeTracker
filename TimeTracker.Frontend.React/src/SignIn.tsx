import React, { useState } from 'react';
import Cookies from 'js-cookie';
// @ts-ignore
import { executePost } from './helpers/fetch'; 

interface SignInProps {
    setIsSignedIn: React.Dispatch<React.SetStateAction<boolean>>;
}

const SignIn: React.FC<SignInProps> = ({ setIsSignedIn }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [rememberMe, setRememberMe] = useState(false);

    const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setEmail(e.target.value);
    };

    const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(e.target.value);
    };

    const handleRememberMeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setRememberMe(e.target.checked);
    };

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        async function fetchData() {      
            let tokenResponse = await executePost(`${import.meta.env.VITE_BASE_URL}/auth/signin`, { email, password, rememberMe });

            Cookies.set('token', tokenResponse.token, { expires: 365 });
            if(rememberMe)
            {
                Cookies.set('rememberMe', tokenResponse.rememberMeToken, { expires: 365 })
            }
            setIsSignedIn(true);
        }
        fetchData();
    };

    return (
        <div className="d-flex justify-content-center align-items-center min-vh-100 bg-dark">
            <div className="card shadow-lg bg-dark text-light border-secondary" style={{ width: '400px' }}>
                <div className="card-body p-4">
                    <h2 className="card-title text-center mb-4 text-light">Sign In</h2>
                    <form onSubmit={handleSubmit}>
                        <div className="mb-3">
                            <label htmlFor='email' className="form-label">Email:</label>
                            <input 
                                type='email' 
                                id='email' 
                                name='email' 
                                className="form-control" 
                                value={email} 
                                onChange={handleEmailChange} 
                                required 
                                placeholder="Enter your email"
                            />
                        </div>
                        <div className="mb-3">
                            <label htmlFor='password' className="form-label">Password:</label>
                            <input 
                                type='password' 
                                id='password' 
                                name='password' 
                                className="form-control" 
                                value={password} 
                                onChange={handlePasswordChange} 
                                required 
                                placeholder="Enter your password"
                            />
                        </div>
                        <div className="mb-3 form-check">
                            <input 
                                type='checkbox' 
                                id='rememberMe'
                                className="form-check-input" 
                                onChange={handleRememberMeChange} 
                            />
                            <label className="form-check-label" htmlFor='rememberMe'>
                                Remember me
                            </label>
                        </div>
                        <button type='submit' className="btn btn-primary w-100">Sign In</button>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default SignIn;