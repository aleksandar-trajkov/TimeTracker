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
            let tokenResponse = await executePost(`${import.meta.env.VITE_BASE_URL}/auth/login`, { email, password, rememberMe });

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
        <div>
            <h2>Sign In</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor='email'>Email:</label>
                    <input type='email' id='email' name='email' value={email} onChange={handleEmailChange} required />
                </div>
                <div>
                    <label htmlFor='password'>Password:</label>
                    <input type='password' id='password' name='password' value={password} onChange={handlePasswordChange} required />
                </div>
                <div>
                    <label>
                        <input type='checkbox' onChange={handleRememberMeChange} />
                        Remember me
                    </label>
                </div>
                <button type='submit'>Sign In</button>
            </form>
        </div>
    );
};

export default SignIn;