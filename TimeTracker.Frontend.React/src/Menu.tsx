import React from 'react';
import { Link } from 'react-router-dom';

const Menu: React.FC = () => (
    <nav className='main-menu'>
        <ul>
            <li><Link to="/">Home</Link></li>
            <li><Link to="/reports">Reports</Link></li>
            <li><Link to="/categories">Categories</Link></li>
            <li><Link to="/settings">Settings</Link></li>
        </ul>
    </nav>
);

export default Menu;