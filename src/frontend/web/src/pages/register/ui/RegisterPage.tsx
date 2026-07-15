import {useEffect, useState} from "react";
import {RegisterForm} from "@/widgets/register-form";
import {type ActionFunctionArgs, useActionData, useNavigate, useNavigation} from "react-router-dom";
import {prepareRegistration} from "@/shared/security";
import {backendBaseUrl} from "@/shared/config";
import {BackupCodesDialog} from "@/features/backup-codes";

export async function registerAction({request}: ActionFunctionArgs) {
    const formData = await request.formData();
    const username = formData.get("username") as string;
    const email = formData.get("email") as string;
    const password = formData.get("password") as string;
    const confirmPassword = formData.get("confirm-password") as string;
    const url = new URL(request.url);
    const registerInviteCode = url.searchParams.get("code");

    if (!registerInviteCode) {
        return {error: {errors: {body: ["An invite code is required to register."]}}};
    }

    if (!username && !email || !password) {
        return {error: {errors: {body: ["Username or email and password are required"]}}};
    }

    if (password !== confirmPassword) {
        return {error: {errors: {body: ["Passwords don't match"]}}};
    }

    if (password.length < 12) {
        return {error: {errors: {body: ["Password must be at least 12 characters long"]}}};
    }

    try {
        const cryptoPayload = await prepareRegistration(username || email, password);

        const response = await fetch(`${backendBaseUrl}/api/Authenticate/register`, {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({
                username,
                email,
                salt: cryptoPayload.srpSaltBase64,
                verifier: cryptoPayload.srpVerifierBase64,
                encryptedMasterKey: cryptoPayload.encryptedMasterKeyBase64,
                keySalt: cryptoPayload.keySaltBase64,
                recoveryEncryptedMasterKey: cryptoPayload.recoveryEncryptedMasterKeyBase64,
                rootFolderKey: cryptoPayload.rootFolderEncryptedKeyBase64,
                registerInviteCode: registerInviteCode
            })
        });

        if (!response.ok) {
            const errorText = await response.text();
            return {error: {errors: {body: [errorText || "Invalid invite code or registration failed."]}}};
        }

        return { success: true, recoveryCode: cryptoPayload.recoveryPasswordPlaintext, username: username };
    } catch (error: unknown) {
        const errorMessage = error instanceof Error ? error.message : "Failed to create account";
        return {error: {errors: {body: [errorMessage]}}};
    }
}

export function RegisterPage() {
    const actionData = useActionData<typeof registerAction>();
    const navigation = useNavigation();
    const navigate = useNavigate();
    const isSubmitting = navigation.state === "submitting";
    const errorMessage = actionData?.error?.errors?.body?.[0];
    const [showRecovery, setShowRecovery] = useState(false);

    useEffect(() => {
        document.title = "Register - noKeyCloud";
    }, []);

    useEffect(() => {
        if (actionData?.success && actionData?.recoveryCode) {
            setShowRecovery(true);
        }
    }, [actionData]);

    const handleRecoveryAcknowledged = () => {
        setShowRecovery(false);
        navigate("/login");
    };

    if (showRecovery && actionData?.recoveryCode) {
        return (
            <BackupCodesDialog
                open={true}
                username={actionData.username}
                onAcknowledge={handleRecoveryAcknowledged}
                codes={[actionData.recoveryCode]}
            />
        );
    }

    return (
        <div className="flex flex-1 w-full items-center justify-center p-6 md:p-10">
            <div className="w-full max-w-sm">
                <RegisterForm error={errorMessage} isSubmitting={isSubmitting}/>
            </div>
        </div>
    )
}