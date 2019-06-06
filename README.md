# GEARS-XR: Collaborative Mixed Reality Platform for Scientific Visualization

<div align="center">
  <img src="https://github.com/USCCACS/GEARS-XR/blob/master/Resources/fig1.png" width=60%><br><br>
</div>


## Summary
Visualization plays a critically important role in modern scientific research project. It is a very powerful tool to express results from observation and experiments, and visually provides scientists with an intuitive overview of the results. So any research project would not go without visualization, like plotting graph and it is often the case that scientists plots data in many different ways with many different axes to extract some meaningful phenomena. Although a simple graph can only express data with two axes, thanks to the development of computer graphics techniques, scientists can also easily visualize data on 3D space by using visualization software. However, as long as plotted on a flat display, data are reduced onto two dimensional and scientists could miss some information inherent in the data, especially in the field targeting at objects with complex geometries or a large number of entities, like material science.

## Environment
We used “Acer Windows Mixed Reality Headset with Motion Controllers,” which has 1440x1440 resolution and 90 fps at maximum, as a head-mounted display. Each users’ headset is connected to a gaming laptop (Predator 17 X, GX-792). Speaking of software aspect, we developed all the software on the Unity 2017 game development platform, using Microsoft Mixed Reality Toolkit as a library. Concerning the networking part, our system highly depends on UNET; networking APT provided from Unity.

## Pipeline
GEARS-XR consist of three components, LAMMPS interface, multi-user component, and the rendering component. LAMMPS is a molecular dynamics simulator library, which is widely used in material science research. As is often the case in material science, a large number of atoms are involved in LAMMPS simulations. Different from any other applications, e.g., games or movies, scientific visualization require strict criteria for preciseness, which makes multi-user component and rendering component non-trivial technical building blocks.

# Sample Use Cases (VR & AR)

## Interactive Viewing
The most straightforward application of our application on mixed reality headsets as well as hololens is interactive viewing of pre- computed results. To demonstrate this aspect of GEARS, we use the results from precomputed atom MD simulation. The position of every molecule is calculated at each step. We isolate one step, or frame, of this simulation and import their coordinates to be rendered by our pipeline.
This aspect provides a quick, straightforward outlet for immediate visualization of data from HPC simulations or structure files like those found on the RCS Protein Data Bank. This process has been expanded to create multiple scenes, each containing the results from a different time step. Then the simulation could be replayed dynamically frame by frame.

## Real-time Simulation

Once a simulation of interest has been coded, it still needs to be incorporated with the engine’s rendering mechanics. This relation can be programmed as the user sees fit. However, to minimize the amount of coding necessary, we present two methods by which simulations can be run and rendered in real-time. One involves running a timestep on a frame update call, then updating particles states on the same frame. We call this method, Run-and-Render. This method offloads the timestep computation onto a new thread, while the main game thread, responsible for handling the render state of the engine, maintains the simulation state of the previous timestep. When the timestep is finished computing, the game thread can either appropriately update the particle states or store them for rendering later while another thread continues to calculate future timesteps. We call this method Render-when-Ready.

This sample is available for both mixed reality and hololens. This real time visualization of simulations is enabled using a massively parallel moleculr dynamics simulator (LAMMPS). LAMMPS is an open source software with a user base of more than 250, 000 around the globe. We have achieved the real-time visualization by compiling lammps as a dll, then wrapping the dll around C# to be read and understood by .NET framework. This process helps achieve simulations of upto 30000 atoms on two threads on gaming laptop (Acer Predator 17 X, GX-792).

## Multi-User Feature
We have implemented multi-user feature with the server-client model through the internet. Once all the users are connected through the internet, one user loads the objects they want to discuss, and the system automatically sends the information of the objects to all the client users. Here, the technical challenge for this multi-user environment is how to realize this transaction efficiently. VR interface requires us to render objects for each user in real-time because each user moves their head to observe what he/she wants to investigate, which inevitably becomes a real-time interaction. The transaction here has to be reduced well to preserve the efficiency of the rendering process. We minimize the information to send as much as possible. What is essential in LAMMPS is to simulate the collective dynamics or structure of a large number of atoms. Synchronization can be realized just by sharing each atom position. Every other aspect of visualization, e.g., VR space environment, particle geometries, particle properties and so on can be synchronized beforehand. So once the server-user loaded the objects, the server system sends all information except for position before the actual synchronization starts.

This feature is only available for mixed reality headsets

# Demos
## Visualization of complex 3D structures
<div align="center">
  <img src="https://github.com/USCCACS/GEARS-XR/blob/master/Resources/nanoCarbon%20(1).PNG" width=60%><br><br>
</div>

## Application Demo
 [![App-Demo](https://img.youtube.com/vi/cX5vcuuTM88/0.jpg)](https://www.youtube.com/watch?v=cX5vcuuTM88)
 
## Graphene Folding Demo
This demonstration shows the capability of our application to visualize series of data frames interactively
[![GrapheneFolding](https://img.youtube.com/vi/sZSGiU1FP5w/0.jpg)](https://www.youtube.com/watch?v=sZSGiU1FP5w)

## Kirigami unfolding visualization
This demonstration shows visualization of complex geometries like Kirigami unfolding
[![KirigamiUnfolding](https://img.youtube.com/vi/wpYFg3uoPMY/0.jpg)](https://www.youtube.com/watch?v=wpYFg3uoPMY)


# Future work
We have extended the capability of our application for Windows Mixed Reality device to Hololens 1st generation. Since mixed reality platform provides easy portability to hololens, with little modifications we were able to extend our visualization framework to hololens.
We further intend to develop an augmented reality application for enhancing 3D modelling of molecular structures, and visualization of atomic structures of various elements, compounds in a spatial surrounding. Our proposed framework will use materials project API [11] at the backend and will use gesture guided moved from the user to detect objects and images in the real world. These objects and image information are then relayed to Azure Cognitive Services to deconstruct these images, objects to their respective text-based structure identification. The structure identity is then queried to materials project API, to pull the information related to its 3D visualization, structure identification along with relevant property information. This will greatly improve an intuitive understanding of materials structure and property which can further aid in future education.

# Requested Features
* *Improving rendering capability*

For increasing the rendering capability of our application, such that it can scale to visualization of a million atoms, we have tried a few things such as bill boarding, GPU-Instancing, level of detail (LOD) rendering. While GPU instancing and LOD does result in performance boosts, we do not achieved any performance gains by doing bill boarding. Work needs to be done, to understand what causes this phenomenon on mixed reality headsets and hololens.

* *AR multi-user feature*

The multi-user feature implemented here for virtual reality using network sharing protocol cannot be identically run on hololens. Since, the sharing feature supported with hololens doesn't scales well with intensive atomic geometries.

* *Educational framework based on augmented reality for 3D molecules and structure visualization*

AR techonolgies can be very effective in materials science and chemistry education for visualization and study of atoms, molecules, crystal lattices. There is a need for AR tools to support education at schools and universities. There is a need for following features to support such a framework:

  * Using materials project API at the backend use gesture guided moves from the user to detect objects and images in the real world. These objects and image information are then relayed to Azure Cognitive Services to deconstruct these images, objects to their respective text-based structure identification. The structure identity is then queried to materials project API, to pull the information related to its 3D visualization, structure identification along with relevant property information.
  
  * 3D visualization of molecules, their dynamics and interaction possibility to form molecules from individual fragments.
  
  

## Acknowledgments
The work on mixed reality devices was supported by Microsoft MR Academic Program.



## Authors
* Ankit Mishra (University of Southern California)
* Rohan Rout (University of Southern California, now at MATLAB)
* Vaibhav Desai (University of Southern California, now at CISCO)
* Hikaru Ibayashi (University of Southern California)
* Taufeq Razakh (University of Southern California)
* Kuang Liu (University of Southern California)

## Published works
Game-Engine-Assisted Research platform for Scientific computing (GEARS) in virtual reality", 
B. K. Horton, R. K. Kalia, E. Moen, A. Nakano, K. Nomura, M. Qian, P. Vashishta, and A. Hafreager
*SoftwareX* 9, 112-116  (2019)
https://www.sciencedirect.com/science/article/pii/S2352711018300633

