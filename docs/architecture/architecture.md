# Architecture

> [!NOTE]
> Later move sections to separate files if needed and link them here.

## 1. System Overview

### Main Goals
- Provide zero-knowledge storage (server cannot read user data)
- Store all files in encrypted form
- Enable cross-device synchronization (web, mobile, desktop)
- Support self-hosting for full infrastructure control
- Maintain modularity and scalability

### Architecture style
- Clean Architecture
- Backend API separated into Domain, Application, and Infrastructure layers
- Clients are independent UI applications

> [!IMPORTANT]
> Add high-level diagram(s) here to illustrate the system architecture and component interactions. This can be a simple block diagram showing major components and their relationships.

---

## 2. Architecture Diagram(s)

> [!IMPORTANT]
> Insert Diagrams

### Context Diagram

### Container Diagram

### Component Diagram (Backend)

---

## 3. Core Components

### Frontend

> [!IMPORTANT]
> NOT DECIDED YET: The frontend architecture will be determined based on the chosen frameworks and platforms (e.g., React for web, React Native for mobile, Electron for desktop). The frontend will interact with the backend API to perform all operations, including file uploads/downloads, metadata management, and synchronization.

### Backend
- REST API
- Built using Clean Architecture
    - Domain: core entities
    - Application: business use cases
    - Infrastructure: PostgreSQL, storage, encryption, caching
- Handles metadata only (never plaintext file content)
- Manages authentication and device registration

### Database
- Type: Relational (PostgreSQL)
    - stores metadata (file structure, users, permissions)
    - store encrypted file references
    - stores encryption keys (if needed, with proper security controls)
- Important
    - No file contents are stored in plaintext or directly in DB

### Storage

> [!IMPORTANT]
> NOT DECIDED YET: The storage solution will be determined based on the requirements for scalability, reliability, and cost. Options include self-hosted object storage (e.g., MinIO), cloud-based storage services (e.g., AWS S3, Azure Blob Storage), or a local file system abstraction. The chosen solution must support secure storage of encrypted files and efficient retrieval.

### Caching

> [!IMPORTANT]
> NOT DECIDED YET: The caching strategy will be determined based on the performance requirements of the system. Options include in-memory caching (e.g., Redis) for frequently accessed metadata and file references, or a distributed caching solution if scalability is a concern. The caching layer will be designed to improve response times while ensuring data consistency and security.

### External Services
- self-hosted JWT

---

## 4. Architecture Style

### Chosen Style
- Clean Architecture
- E2EE-first design

### Reasoning
- Strong separation of concerns
- Easy testing of domain logic
- Encryption isolated from infrastructure
- Flexible deployment (self-hosted friendly)

### Advantages
- High maintainability
- Replaceable infrastructure components
- Strong security boundaries

### Disadvantages
- Higher initial complexity
- More boilerplate code
- Requires strict discipline in structure

---

## 5. Communication Between Components

- Api style: REST (primary)
- Communication type:
    - synchronous: API requests (auth, metadata, file operations)
    - asynchronous: sync updates, background jobs
- Data formats
    - JSON for API communication
    - Encrypted binary for file storage

---

## 6. Data Model (High-Level)

> [!IMPORTANT]
> Add high-level ER diagram or class diagram illustrating key entities (User, File, Device, etc.) and their relationships. Focus on how metadata is structured and how encrypted file references are managed.

---

## 7. Scalability & Performance

### Scaling strategy
- Horizontal scaling for backend - stateless API (more resources)
- Database scaling - read replicas, partitioning if needed
- Storage scaling - object storage with auto-scaling capabilities

### Performance optimizations
- Caching frequently accessed metadata
- Asynchronous processing for file uploads/downloads
- Efficient indexing in the database for metadata queries
- Streaming for large files (no full memory loading)
- Chunked file upload/download
- Delta-based sync instead of full file transfers
- Client-side encryption overhead
- Deduplication (future optimization)


