import './App.css';
import Navbar from './components/navbar/Navbar.js'

function App() {
    return (
        <div className="container-fluid px-0">
            <Navbar
                activePage='DASHBOARD'
                isUserLoggedIn={true}
                username='miscecut'
            />
        </div>
    );
}

export default App;