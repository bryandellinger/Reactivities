import React from 'react';
import {Container, Menu, Button } from 'semantic-ui-react'
import { useStore } from '../stores/store';



export default function Navbar(){

    const {activityStore} = useStore();
    return(
       <Menu inverted fixed='top'>
            <Container>
                 <Menu.Item header>
                    <img src="/assests/logo.png" alt="logo" style={{marginRight: 10}}/>
                    Reactivities
                </Menu.Item>
                <Menu.Item name="Activities"/>
                <Menu.Item>
                    <Button positive content='create activity'
                     onClick={() => activityStore.openForm()}/>
                </Menu.Item>
          
            </Container>
       </Menu>  
    )
}