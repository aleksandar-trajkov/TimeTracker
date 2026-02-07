import React from 'react';
import Menu from './Menu';

const Layout: React.FC<React.PropsWithChildren<object>> = ({ children }) => {
    return (
        <div className='body-container'>
            <div className='header'>
                <Menu />
            </div>
            <div className='main p-3'>
                {children}
            </div>
            <div className='footer'></div>
        </div>
    );
};

export default Layout